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
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using ConfigService.DALSQLHelper;

namespace Trade.DALSQLAzure.Federations
{
    /// <summary>
    /// Helper class to handle changing to correct Federation members.
    /// </summary>
    class Federation
    {
        private const string UseFederationFormat = "USE FEDERATION {0} ({1} = {2}) WITH RESET, FILTERING = {3}";
        private const string UseFederationRoot = "USE FEDERATION ROOT WITH RESET";

        private readonly SqlConnection _internalConnection;
        private readonly string _federationName;
        private readonly string _federationKey;
        private readonly FederationKeyType _federationKeyType;

        private object _federationKeyValue;
        private bool _isFederationFiltered;

        /// <summary>
        /// Initializes a new instance of the <see cref="Federation"/> class.
        /// </summary>
        public Federation(SqlConnection internalConnection, string federationName, string federationKey, FederationKeyType federationKeyType)
        {
            _internalConnection = internalConnection;
            _federationName = federationName;
            _federationKey = federationKey;
            _federationKeyType = federationKeyType;
        }

        /// <summary>
        /// Gets the last statement used to adjust the selected federation member.
        /// </summary>
        public string LastStatement { get; private set; }

        /// <summary>
        /// Selects the specified federation member and then enlists in the current transaction (if available).
        /// </summary>
        /// <param name="keyValue">The federation key value.</param>
        /// <param name="isFiltered">Whether federation connection is filtered.</param>
        public bool Select(object keyValue, bool isFiltered)
        {
            bool changed = false;
            if (_federationKeyValue != null && (!string.Equals(keyValue, _federationKeyValue) || _isFederationFiltered != isFiltered))
            {
                // Allow changing federation if new symbol/filtering settings are specified
                _federationKeyValue = null;
            }

            if (_federationKeyValue == null)
            {
                LastStatement = string.Format(CultureInfo.InvariantCulture,
                                              UseFederationFormat,
                                              _federationName,
                                              _federationKey,
                                              GetKeyValue(keyValue),
                                              isFiltered ? "ON" : "OFF");
                SQLHelper.ExecuteNonQuery(_internalConnection, null, CommandType.Text, LastStatement);

                _federationKeyValue = keyValue;
                _isFederationFiltered = isFiltered;

                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Resets to the root federation member.
        /// </summary>
        public bool Reset()
        {
            bool changed = false;
            if (_federationKeyValue != null)
            {
                _federationKeyValue = null;
                _isFederationFiltered = false;

                LastStatement = UseFederationRoot;
                SQLHelper.ExecuteNonQuery(_internalConnection, null, CommandType.Text, LastStatement);

                changed = true;
            }

            return changed;
        }

        /// <summary>
        /// Formats the key value for use in the USE FEDERATION statement.
        /// </summary>
        /// <param name="keyValue">The key value.</param>
        /// <returns></returns>
        private string GetKeyValue(object keyValue)
        {
            switch (_federationKeyType)
            {
                case FederationKeyType.Integer:
                    if (!(keyValue is int) && !(keyValue is uint))
                    {
                        throw new ArgumentOutOfRangeException("keyValue");
                    }

                    return keyValue.ToString();
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
