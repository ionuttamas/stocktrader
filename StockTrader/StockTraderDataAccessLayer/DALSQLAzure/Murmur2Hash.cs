//  .NET StockTrader Sample WCF Application for Benchmarking, Performance Analysis and Design Considerations for Service-Oriented Applications
//                   3/1/2012: Updated to version 6.0, with notable enhancements for Windows Azure hosting and mobile compatibility. See: 
//                                  1. Technical overview paper: https://azurestocktrader.blob.core.windows.net/docs/Trade6Overview.pdf
//                                  2. MSDN Site with downloads, additional information: http://msdn.microsoft.com/stocktrader
//                                  3. Discussion Forum: http://social.msdn.microsoft.com/Forums/en-US/dotnetstocktradersampleapplication
//                                  4. Live on Windows Azure: https://azurestocktrader.cloudapp.net
//  

using System;

namespace Trade.DALSQLAzure
{
    /// <summary>
    /// Implementation of MurmurHash. More details here: http://en.wikipedia.org/wiki/MurmurHash
    /// </summary>
    public static class Murmur2Hash
    {
        private const uint M = 0x5bd1e995;
        private const int R = 24;
        private const uint Seed = 0x9747b28c;

        public static uint Hash(byte[] data)
        {
            uint hash = 0;
            if (data.Length > 0)
            {
                hash = HashData(data);
            }

            return hash;
        }

        private static uint HashData(byte[] data)
        {
            var hash = Seed ^ (uint) data.Length;

            var index = 0;
            for (index = 0; index <= data.Length - 4; index += 4)
            {
                var k = BitConverter.ToUInt32(data, index);

                k *= M;
                k ^= (k >> R);
                k *= M;

                hash *= M;
                hash ^= k;
            }

            var remainder = data.Length - index;
            if (remainder > 0)
            {
                switch (remainder)
                {
                    case 0:
                        break;
                    case 1:
                        hash ^= data[index];
                        break;
                    case 2:
                        hash ^= BitConverter.ToUInt16(data, index);
                        break;
                    case 3:
                        hash ^= BitConverter.ToUInt16(data, index);
                        hash ^= (uint) data[index + 2] << 16;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                hash *= M;
            }

            hash ^= (hash >> 13);
            hash *= M;
            hash ^= (hash >> 15);

            return hash;
        }
    }
}
