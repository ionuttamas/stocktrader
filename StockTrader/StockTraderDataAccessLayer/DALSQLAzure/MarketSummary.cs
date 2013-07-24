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

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using ConfigService.DALSQLHelper;
using Trade.BusinessServiceDataContract;
using Trade.IDAL;
using Trade.Utility;

namespace Trade.DALSQLAzure
{
    /// <summary>
    /// Market Summary implementation for SQL Azure.
    /// </summary>
    public class MarketSummary : IMarketSummary
    {
        private const string SQL_SELECT_QUOTE = "SET NOCOUNT ON; SELECT symbol, companyname, volume, price, open1, low, high, change1 from dbo.quote with (ROWLOCK) where symbol = @QuoteSymbol";
        private const string SQL_SELECT_QUOTE_NOLOCK = "SET NOCOUNT ON; SELECT symbol, companyname, volume, price, open1, low, high, change1 from dbo.quote with (NOLOCK) where symbol = @QuoteSymbol";
        protected const string SQL_SELECT_QUOTES_NOLOCK = "SET NOCOUNT ON; SELECT symbol, companyname, volume, price, open1, low, high, change1 from dbo.quote with (NOLOCK) where symbol in ({0})";
        private const string SQL_UPDATE_STOCKPRICEVOLUME = "SET NOCOUNT ON; UPDATE dbo.QUOTE WITH (ROWLOCK) SET PRICE=@Price, Low=@Low, High=@High, Change1 = @Price - open1, VOLUME=VOLUME+@Quantity WHERE SYMBOL=@QuoteSymbol";
        private const string SQL_INSERT_QUOTEHISTORY = "SET NOCOUNT ON; INSERT INTO QUOTEHISTORY VALUES (@QuoteHistoryId, @OrderId, @QuoteSymbol, @QuoteSymbolHash, @Timestamp, @Quantity, @Price)";

        //Parameters
        private const string PARM_SYMBOL = "@QuoteSymbol";
        private const string PARM_SYMBOLHASH = "@QuoteSymbolHash";
        private const string PARM_PRICE = "@Price";
        private const string PARM_LOW = "@Low";
        private const string PARM_HIGH = "@High";
        private const string PARM_QUANTITY = "@Quantity";
        private const string PARM_QUOTEHISTORYID = "@QuoteHistoryId";
        private const string PARM_ORDERID = "@OrderId";
        private const string PARM_TIMESTAMP = "@Timestamp";

        //_internalConnection: Used by a DAL instance such that a DAL instance,
        //associated with a BSL instance, will work off a single connection between BSL calls.
        protected SqlConnection _internalConnection;

        //Used only when doing ADO.NET transactions.
        //This will be completely ignored when null, and not attached to a cmd object
        //In SQLHelper unless it has been initialized explicitly in the BSL with a
        //dal.BeginADOTransaction().  See app config setting in web.config and 
        //Trade.BusinessServiceHost.exe.config "Use System.Transactions Globally" which determines
        //whether user wants to run with ADO transactions or System.Transactions.  The DAL itself
        //is built to be completely agnostic and will work with either.
        protected SqlTransaction _internalADOTransaction;

        private readonly SqlAzureHelper _helper;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketSummary"/> class.
        /// </summary>
        public MarketSummary()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor for internal DAL-DAL calls to use an existing DB connection.
        /// </summary>
        protected internal MarketSummary(SqlConnection conn, SqlTransaction transaction)
            : this(conn, transaction, new SqlAzureHelper())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MarketSummary"/> class.
        /// </summary>
        protected MarketSummary(SqlConnection conn, SqlTransaction transaction, SqlAzureHelper helper)
        {
            _internalConnection = conn;
            _internalADOTransaction = transaction;
            _helper = helper;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public virtual void BeginADOTransaction()
        {
            _internalADOTransaction = _internalConnection.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public virtual void RollBackTransaction()
        {
            if (_internalADOTransaction != null)
            {
                _internalADOTransaction.Rollback();
            }

            _internalADOTransaction = null;
        }

        /// <summary>
        /// Used only when doing ADO.NET transactions.
        /// </summary>
        public virtual void CommitADOTransaction()
        {
            if (_internalADOTransaction != null)
            {
                _internalADOTransaction.Commit();
            }

            _internalADOTransaction = null;
        }

        /// <summary>
        /// Opens the connection using the specified connnection string.
        /// </summary>
        /// <param name="connString">The connection string.</param>
        public virtual void Open(string connString)
        {
            if (_internalConnection == null)
            {
                _internalConnection = _helper.Create(connString);
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            if (_internalConnection != null && _internalConnection.State != ConnectionState.Closed)
            {
                _internalConnection.Close();
            }
        }

        /// <summary>
        /// Gets the quote for a specified symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public virtual QuoteDataModel getQuote(string symbol)
        {
            SqlParameter parm1 = new SqlParameter(PARM_SYMBOL, SqlDbType.VarChar, 10)
                                     {Value = symbol};

            QuoteDataModel quote = null;
            using (SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_QUOTE_NOLOCK, parm1))
            {
                if (rdr.HasRows)
                {
                    rdr.Read();
                    quote = new QuoteDataModel(symbol, rdr.GetString(1), rdr.GetDouble(2), rdr.GetDecimal(3),
                                               rdr.GetDecimal(4), rdr.GetDecimal(5), rdr.GetDecimal(6), rdr.GetDouble(7));
                }
            }

            return quote;
        }

        /// <summary>
        /// Gets quotes for the specified symbols.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        public virtual List<QuoteDataModel> getQuotes(string symbols)
        {
            List<QuoteDataModel> quotes = symbols.Split(new[] { ' ', ',', ';' }).Select(x => new QuoteDataModel { symbol = x }).ToList();

            List<SqlParameter> parameters = new List<SqlParameter>();
            for (int i = 0; i < quotes.Count; i++)
            {
                QuoteDataModel current = quotes[i];
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
                    QuoteDataModel quoteDataModel = quotes.First(x => x.symbol == symbol);
                    quoteDataModel.companyName = rdr.GetString(1);
                    quoteDataModel.volume = rdr.GetDouble(2);
                    quoteDataModel.price = rdr.GetDecimal(3);
                    quoteDataModel.open = rdr.GetDecimal(4);
                    quoteDataModel.low = rdr.GetDecimal(5);
                    quoteDataModel.high = rdr.GetDecimal(6);
                    quoteDataModel.change = rdr.GetDouble(7);
                }
            }

            return quotes;
        }

        /// <summary>
        /// Gets the quote (for update).
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        public virtual QuoteDataModel getQuoteForUpdate(string symbol)
        {
            SqlParameter parm1 = new SqlParameter(PARM_SYMBOL, SqlDbType.VarChar, 10)
                                     {Value = symbol};

            SqlDataReader rdr = _helper.ExecuteReaderSingleRowSingleParm(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_SELECT_QUOTE, parm1);
            QuoteDataModel quote = null;
            if (rdr.HasRows)
            {
                rdr.Read();
                quote = new QuoteDataModel(symbol, rdr.GetString(1), rdr.GetDouble(2), rdr.GetDecimal(3), rdr.GetDecimal(4), rdr.GetDecimal(5), rdr.GetDecimal(6), rdr.GetDouble(7));
            }
                
            rdr.Close();
            return quote;
        }

        /// <summary>
        /// Updates the stock price volume.
        /// </summary>
        /// <param name="quantity">The quantity.</param>
        /// <param name="quote">The quote.</param>
        public virtual void updateStockPriceVolume(double quantity, QuoteDataModel quote)
        {
            decimal priceChangeFactor = StockTraderUtility.getRandomPriceChangeFactor(quote.price);
            decimal newprice = quote.price * priceChangeFactor;
            if (newprice < quote.low)
                quote.low = newprice;
            if (newprice > quote.high)
                quote.high = newprice;
            
            SqlParameter[] updatestockpriceparm = GetUpdateStockPriceVolumeParameters();
            updatestockpriceparm[0].Value = newprice;
            updatestockpriceparm[1].Value = (float)quantity;
            updatestockpriceparm[2].Value = quote.symbol;
            updatestockpriceparm[3].Value = quote.low;
            updatestockpriceparm[4].Value = quote.high;

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text, SQL_UPDATE_STOCKPRICEVOLUME, updatestockpriceparm);
        }

        /// <summary>
        /// Inserts the stock history.
        /// </summary>
        /// <param name="order">The order.</param>
        public virtual void insertStockHistory(OrderDataModel order)
        {
            SqlParameter[] parameters = new[]
                                 {
                                     new SqlParameter(PARM_QUOTEHISTORYID, SqlDbType.UniqueIdentifier)
                                         {Value = Guid.NewGuid()},
                                     new SqlParameter(PARM_ORDERID, SqlDbType.UniqueIdentifier) {Value = Guid.Parse(order.orderID)},
                                     new SqlParameter(PARM_SYMBOL, SqlDbType.VarChar, 10)
                                         {Value = order.symbol},
                                     new SqlParameter(PARM_SYMBOLHASH, SqlDbType.Int)
                                         {Value = order.symbol.GetSignedMurmur2HashCode()}, 
                                     new SqlParameter(PARM_TIMESTAMP, SqlDbType.DateTime) {Value = DateTime.UtcNow},
                                     new SqlParameter(PARM_QUANTITY, SqlDbType.Float) {Value = order.quantity},
                                     new SqlParameter(PARM_PRICE, SqlDbType.Decimal, 14) {Value = order.price}
                                 };

            _helper.ExecuteNonQuery(_internalConnection, _internalADOTransaction, CommandType.Text,
                                      SQL_INSERT_QUOTEHISTORY, parameters);
        }

        /// <summary>
        /// Gets the market summary data.
        /// </summary>
        public MarketSummaryDataModelWS getMarketSummaryData()
        {
            IEnumerable<string> symbols = Enumerable.Range(0, 100).Select(x => string.Format(CultureInfo.InvariantCulture, "s:1{0}", x.ToString("00", CultureInfo.InvariantCulture)));
            List<QuoteDataModel> quotes = getQuotes(string.Join(",", symbols)).ToList();

            return new MarketSummaryDataModelWS(
                quotes.Sum(x => x.price) / Math.Max(1, quotes.Count),
                quotes.Sum(x => x.open) / Math.Max(1, quotes.Count),
                quotes.Sum(x => x.volume),
                quotes.OrderByDescending(x => x.change).Take(5).ToList(),
                quotes.OrderBy(x => x.change).Take(5).ToList());
        }

        /// <summary>
        /// Gets the update stock price volume parameters.
        /// </summary>
        private static SqlParameter[] GetUpdateStockPriceVolumeParameters()
        {
            // Get the paramters from the cache
            SqlParameter[] parms = SQLHelper.GetCacheParameters(SQL_UPDATE_STOCKPRICEVOLUME);
            
            // If the cache is empty, rebuild the parameters
            if (parms == null)
            {
                parms = new[] {
                            new SqlParameter(PARM_PRICE, SqlDbType.Decimal, 14),
                            new SqlParameter(PARM_QUANTITY, SqlDbType.Float),
                            new SqlParameter(PARM_SYMBOL, SqlDbType.VarChar, 10),
                            new SqlParameter(PARM_LOW, SqlDbType.Decimal, 14),
                            new SqlParameter(PARM_HIGH, SqlDbType.Decimal, 14)};
                
                // Add the parametes to the cached
                SQLHelper.CacheParameters(SQL_UPDATE_STOCKPRICEVOLUME, parms);
            }

            return parms;
        }
    }
}
