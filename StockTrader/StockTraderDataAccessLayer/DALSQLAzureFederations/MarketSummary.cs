//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//                                   
//
//  Configuration Service 6 Notes:
//                      The application implements Configuration Service 6.0, for which the source code also ships in the sample. However, the .NET StockTrader 6
//                      sample is a general-purpose performance sample application for Windows Server and Windows Azure even if you are not implementing the Configuration Service. 
//                      
//

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Transactions;
using Trade.BusinessServiceDataContract;
using IsolationLevel = System.Data.IsolationLevel;

namespace Trade.DALSQLAzure.Federations
{
    /// <summary>
    /// Market Summary implementation for SQL Azure with Federations.
    /// </summary>
    public class MarketSummary : DALSQLAzure.MarketSummary
    {
        private readonly SqlAzureFederationsHelper _helper;
        private bool _inAdoTransaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketSummary"/> class.
        /// </summary>
        public MarketSummary()
            : this(null, null, new SqlAzureFederationsHelper())
        {
        }

        /// <summary>
        /// Constructor for internal DAL-DAL calls to use an existing DB connection.
        /// </summary>
        internal MarketSummary(SqlConnection conn, SqlTransaction transaction, SqlAzureFederationsHelper helper)
            : base(conn, transaction, helper)
        {
            _helper = helper;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public override void BeginADOTransaction()
        {
            _inAdoTransaction = true;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public override void RollBackTransaction()
        {
            _inAdoTransaction = false;
            base.RollBackTransaction();
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public override void CommitADOTransaction()
        {
            _inAdoTransaction = false;
            base.CommitADOTransaction();
        }

        /// <summary>
        /// Opens the connection using the specified connnection string.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        public override void Open(string connString)
        {
            base.Open(connString);
            _helper.Initialize(FederationFactory.CreateQuoteFederation(_internalConnection));
        }

        /// <summary>
        /// Gets the quote for a specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public override QuoteDataModel getQuote(string symbol)
        {
            Select(symbol, true);
            return base.getQuote(symbol);
        }

        /// <summary>
        /// Gets quotes for the specified symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        public override List<QuoteDataModel> getQuotes(string symbols)
        {
            List<QuoteDataModel> quotes = symbols.Split(new[] { ' ', ',', ';' }).Select(x => new QuoteDataModel { symbol = x }).ToList();

            // Quotes could be spread across multiple federation members. We'll need to iterate through several. To make
            // this operation as economical as possible, we resolve the federation member for the first symbol and then
            // query the federation member for all symbols. Not all symbols will be present but this approach ensure we don't
            // need to query any federation member more than once.
            while (true)
            {
                List<QuoteDataModel> remainingQuotes = quotes.Where(x => x.companyName == null).ToList();
                if (remainingQuotes.Count == 0)
                {
                    break;
                }

                // Select the federation member for the first remaining quote
                Select(remainingQuotes[0].symbol, false);

                List<SqlParameter> parameters = new List<SqlParameter>();
                for (int i = 0; i < remainingQuotes.Count; i++)
                {
                    QuoteDataModel current = remainingQuotes[i];
                    parameters.Add(new SqlParameter("quote" + i, SqlDbType.VarChar, 10) { Value = current.symbol });
                }

                // Select quote for all remaining quotes
                string command = string.Format(CultureInfo.InvariantCulture, SQL_SELECT_QUOTES_NOLOCK,
                                               string.Join(",", parameters.Select(x => "@" + x.ParameterName)));

                using (SqlDataReader rdr = _helper.ExecuteReader(_internalConnection, _internalADOTransaction,
                                                                 CommandType.Text, command, parameters.ToArray()))
                {
                    while (rdr.Read())
                    {
                        string symbol = rdr.GetString(0);

                        // Update the quote inside the remaining quotes list
                        QuoteDataModel quoteDataModel = remainingQuotes.First(x => x.symbol == symbol);
                        quoteDataModel.companyName = rdr.GetString(1);
                        quoteDataModel.volume = rdr.GetDouble(2);
                        quoteDataModel.price = rdr.GetDecimal(3);
                        quoteDataModel.open = rdr.GetDecimal(4);
                        quoteDataModel.low = rdr.GetDecimal(5);
                        quoteDataModel.high = rdr.GetDecimal(6);
                        quoteDataModel.change = rdr.GetDouble(7);
                    }

                    // If the quote we used to resolve the federation member is not found, then give up on this quote
                    if (remainingQuotes[0].companyName == null)
                    {
                        quotes.Remove(remainingQuotes[0]);
                    }
                }
            }

            return quotes;
        }

        /// <summary>
        /// Gets the quote (for update).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public override QuoteDataModel getQuoteForUpdate(string symbol)
        {
            Select(symbol, true);
            return base.getQuoteForUpdate(symbol);
        }

        /// <summary>
        /// Updates the stock price volume.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="quote">The quote.</param>
        public override void updateStockPriceVolume(double quantity, QuoteDataModel quote)
        {
            Select(quote.symbol, true);
            base.updateStockPriceVolume(quantity, quote);
        }

        /// <summary>
        /// Inserts the stock history.
        /// </summary>
        /// <param name="order">The order.</param>
        public override void insertStockHistory(OrderDataModel order)
        {
            Select(order.symbol, true);
            base.insertStockHistory(order);
        }

        /// <summary>
        /// Selects the federation.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="isFiltered">if set to <c>true</c> [is filtered].</param>
        private void Select(string symbol, bool isFiltered)
        {
            bool changed = symbol == null
                               ? _helper.Reset()
                               : _helper.Select(symbol.GetSignedMurmur2HashCode(), isFiltered);

            if (changed)
            {
                // Connection will already be open and cannot be enlisted in a transaction when issuing 
                // a USE FEDERATION command. This will explicitly enlist the connection in a transaction.
                if (Transaction.Current != null)
                {
                    _internalConnection.EnlistTransaction(Transaction.Current);
                }
                else if (_inAdoTransaction)
                {
                    _internalADOTransaction = _internalConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                }
            }
        }
    }
}
