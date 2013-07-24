//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//  

using System;
using System.Data;
using System.Data.SqlClient;
using ConfigService.DALSQLHelper;

namespace Trade.DALSQLAzure.Federations
{
    class SqlAzureFederationsHelper : SqlAzureHelper
    {
        private Federation _federation;

        /// <summary>
        /// Initializes the object with the specified federation.
        /// </summary>
        public void Initialize(Federation federation)
        {
            Current = federation;
        }

        /// <summary>
        /// Gets or sets the federation.
        /// </summary>
        private Federation Current
        {
            get
            {
                if (_federation == null)
                {
                    throw new InvalidOperationException();
                }

                return _federation;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                if (_federation != null)
                {
                    throw new InvalidOperationException();
                }

                _federation = value;
            }
        }

        /// <summary>
        /// Selects the specified federation member and then enlists in the current transaction (if available).
        /// </summary>
        /// <param name="federationKeyValue">The federation key value.</param>
        /// <param name="isFiltered">Whether federation connection is filtered.</param>
        public bool Select(object federationKeyValue, bool isFiltered)
        {
            if (Current == null)
            {
                throw new InvalidOperationException();
            }

            return Current.Select(federationKeyValue, isFiltered);
        }

        /// <summary>
        /// Resets to the root federation member.
        /// </summary>
        public bool Reset()
        {
            return Current.Reset();
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        public override void ExecuteNonQuery(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParams)
        {
            SQLHelper.ExecuteNonQuery(conn, trans, cmdType, Current.LastStatement, cmdText, cmdParams);
        }

        /// <summary>
        /// Executes the non query single parm.
        /// </summary>
        public override void ExecuteNonQuerySingleParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter singleParam)
        {
            SQLHelper.ExecuteNonQuerySingleParm(conn, trans, cmdType, Current.LastStatement, cmdText, singleParam);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        public override SqlDataReader ExecuteReader(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            return SQLHelper.ExecuteReader(conn, trans, cmdType, Current.LastStatement, cmdText, cmdParms);
        }

        /// <summary>
        /// Executes the reader no parm.
        /// </summary>
        public override SqlDataReader ExecuteReaderNoParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText)
        {
            return SQLHelper.ExecuteReaderNoParm(conn, trans, cmdType, Current.LastStatement, cmdText);
        }

        /// <summary>
        /// Executes the reader single parm.
        /// </summary>
        public override SqlDataReader ExecuteReaderSingleParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter singleParm)
        {
            return SQLHelper.ExecuteReaderSingleParm(conn, trans, cmdType, Current.LastStatement, cmdText, singleParm);
        }

        /// <summary>
        /// Executes the reader single row.
        /// </summary>
        public override SqlDataReader ExecuteReaderSingleRow(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParams)
        {
            return SQLHelper.ExecuteReaderSingleRow(conn, trans, cmdType, Current.LastStatement, cmdText, cmdParams);
        }

        /// <summary>
        /// Executes the reader single row single parm.
        /// </summary>
        public override SqlDataReader ExecuteReaderSingleRowSingleParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter singleParam)
        {
            return SQLHelper.ExecuteReaderSingleRowSingleParm(conn, trans, cmdType, Current.LastStatement, cmdText, singleParam);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        public override void ExecuteScalar(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, params SqlParameter[] cmdParms)
        {
            SQLHelper.ExecuteScalar(conn, trans, cmdType, Current.LastStatement, cmdText, cmdParms);
        }

        /// <summary>
        /// Executes the scalar no parm.
        /// </summary>
        public override void ExecuteScalarNoParm(SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText)
        {
            SQLHelper.ExecuteScalarNoParm(conn, trans, cmdType, Current.LastStatement, cmdText);
        }
    }
}
