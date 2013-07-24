using System;
using System.Transactions;
using Trade.BusinessServiceDataContract;
using Trade.DALFactory;
using Trade.IDAL;
using Trade.OrderProcessorAsyncClient;
using Trade.OrderProcessorContract;
using Trade.OrderProcessorServiceConfigurationSettings;
using Trade.Utility;

namespace Trade.OrderProcessorImplementation
{
    public class OrderProcessorEngine : IOrderProcessor
    {
        public event EventHandler<OrderProcessedEventArgs> OrderProcessed = delegate { };

        /// <summary>
        /// Online check method. No-op.
        /// </summary>
        public void isOnline()
        {
        }

        /// <summary>
        /// SubmitOrder service operation is a service operation that processes order messages from the client
        /// coming in via TCP, HTTP, or a Service Bus WCF binding from either the BSL or another remote instance
        /// </summary>
        /// <param name="order">Order to submit.</param>
        public void SubmitOrder(OrderDataModel order)
        {
            CheckConnection();

            IMarketSummary marketSummary = MarketSummary.Create(Settings.DAL);

            try
            {
                marketSummary.Open(Settings.TRADEDB_SQL_CONN_STRING);

                using (TransactionScope tx = new TransactionScope(TransactionScopeOption.Required, GetTransactionOptions()))
                {
                    // Retrieve the current trade price
                    QuoteDataModel quote = marketSummary.getQuoteForUpdate(order.symbol);
                    order.price = quote.price;

                    // Update market stats
                    marketSummary.updateStockPriceVolume(order.quantity, quote);
                    
                    // Update market history
                    marketSummary.insertStockHistory(order);

                    CheckForAcidTest(order);

                    // Send response. Don't make it part of the transaction as distributed txns not supported.
                    using (new TransactionScope(TransactionScopeOption.Suppress))
                    {
                        SendProcessResponseWithRetry(order);
                    }

                    tx.Complete();
                }
            }
            finally
            {
                marketSummary.Close();
            }
        }

        /// <summary>
        /// Retry submitting the order response in the case of a failure.
        /// </summary>
        private void SendProcessResponseWithRetry(OrderDataModel order)
        {
            const int maxRetry = 3;
            for (int i = 0; i < maxRetry; i++)
            {
                try
                {
                    SendProcessResponse(order);
                    break;
                }
                catch (Exception)
                {
                    if (i == maxRetry - 1)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Send the response outside of the transaction to avoid MSDTC and to ensure txn doesn't lock for longer than
        /// required (leading to timeouts at load).
        /// </summary>
        private void SendProcessResponse(OrderDataModel order)
        {
            OrderResponseDataModel response = new OrderResponseDataModel { orderID = order.orderID, userID = order.userID, price = order.price };
            if (Settings.ORDER_RESPONSE_MODE == StockTraderUtility.ORS_INPROCESS)
            {
                OrderProcessed(this, new OrderProcessedEventArgs(response));
            }
            else
            {
                OrderProcessorResponseAsyncClient client = new OrderProcessorResponseAsyncClient(Settings.ORDER_RESPONSE_MODE, new Settings());
                client.OrderProcessed(response);
            }
        }

        /// <summary>
        /// Get the standard set of transaction options.
        /// </summary>
        private static TransactionOptions GetTransactionOptions()
        {
            return new TransactionOptions
                       {
                           IsolationLevel = IsolationLevel.ReadCommitted,
                           Timeout = TimeSpan.FromSeconds(Settings.SYSTEMDOTTRANSACTION_TIMEOUT)
                       };
        }

        /// <summary>
        /// Throw exception (and rollback transaction) if user trades with specific symbols.
        /// </summary>
        private static void CheckForAcidTest(OrderDataModel order)
        {
            //Now perform our ACID tx test, if requested based on order type and acid stock symbols
            if (order.symbol.Equals(StockTraderUtility.ACID_TEST_BUY) && order.orderType == StockTraderUtility.ORDER_TYPE_BUY)
            {
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_ACID_BUY);
            }

            if (order.symbol.Equals(StockTraderUtility.ACID_TEST_SELL) && order.orderType == StockTraderUtility.ORDER_TYPE_SELL)
            {
                throw new Exception(StockTraderUtility.EXCEPTION_MESSAGE_ACID_SELL);
            }
        }

        /// <summary>
        /// This short try/catch block is introduced to deal with idle-timeout on SQL Azure
        /// connections.  It may not be required in the near future, but as of publication
        /// SQL Azure disconnects idle connections after 30 minutes.  While command retry-logic
        /// in the DAL automatically deals with this, when performing a tx, with the BSL handling
        /// tx boundaries, we want to go into the tx with known good connections.  The try/catch below
        /// ensures this.
        /// </summary>
        private static void CheckConnection()
        {
            IOrder dalOrder = Order.Create(Settings.DAL);
            try
            {
                dalOrder.Open(Settings.TRADEDB_SQL_CONN_STRING);
                dalOrder.getSQLContextInfo();
            }
            catch
            {
            }
            finally
            {
                dalOrder.Close();
            }
        }
    }
}
