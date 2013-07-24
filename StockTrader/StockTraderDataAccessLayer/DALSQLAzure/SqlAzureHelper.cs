//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//  

using System.Data;
using System.Data.SqlClient;
using ConfigService.DALSQLHelper;
using Trade.Utility;

namespace Trade.DALSQLAzure
{
    public class SqlAzureHelper
    {
        /// <summary>
        /// Creates the specified connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public SqlConnection Create(string connectionString)
        {
            return SQLHelper.OpenNewConnection(connectionString, StockTraderUtility.SQL_OPEN_RETRY);
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        public virtual void ExecuteNonQuery(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParams)
        {
            SQLHelper.ExecuteNonQuery(conn, trans, cmdType, cmdText, cmdParams);
        }

        /// <summary>
        /// Executes the non query single parm.
        /// </summary>
        public virtual void ExecuteNonQuerySingleParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter singleParam)
        {
            SQLHelper.ExecuteNonQuerySingleParm(conn, trans, cmdType, cmdText, singleParam);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        public virtual SqlDataReader ExecuteReader(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            return SQLHelper.ExecuteReader(conn, trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// Executes the reader no parm.
        /// </summary>
        public virtual SqlDataReader ExecuteReaderNoParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText)
        {
            return SQLHelper.ExecuteReaderNoParm(conn, trans, cmdType, cmdText);
        }

        /// <summary>
        /// Executes the reader single parm.
        /// </summary>
        public virtual SqlDataReader ExecuteReaderSingleParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter singleParm)
        {
            return SQLHelper.ExecuteReaderSingleParm(conn, trans, cmdType, cmdText, singleParm);
        }

        /// <summary>
        /// Executes the reader single row.
        /// </summary>
        public virtual SqlDataReader ExecuteReaderSingleRow(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParams)
        {
            return SQLHelper.ExecuteReaderSingleRow(conn, trans, cmdType, cmdText, cmdParams);
        }

        /// <summary>
        /// Executes the reader single row single parm.
        /// </summary>
        public virtual SqlDataReader ExecuteReaderSingleRowSingleParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter singleParam)
        {
            return SQLHelper.ExecuteReaderSingleRowSingleParm(conn, trans, cmdType, cmdText, singleParam);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        public virtual void ExecuteScalar(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SQLHelper.ExecuteScalar(conn, trans, cmdType, cmdText, cmdParms);
        }

        /// <summary>
        /// Executes the scalar no parm.
        /// </summary>
        public virtual void ExecuteScalarNoParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText)
        {
            SQLHelper.ExecuteNonQueryNoParm(conn, trans, cmdType, cmdText);
        }
    }
}
