//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//  

using System.Text;

namespace Trade.DALSQLAzure
{
    public static class Murmur2HashExtensions
    {
        public static int GetSignedMurmur2HashCode(this string input)
        {
            return GetSignedMurmur2HashCode(Encoding.UTF8.GetBytes(input));
        }

        public static int GetSignedMurmur2HashCode(this byte[] input)
        {
            return unchecked((int)Murmur2Hash.Hash(input));
        }
    }
}
