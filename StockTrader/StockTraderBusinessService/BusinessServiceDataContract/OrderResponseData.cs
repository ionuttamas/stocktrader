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

using System.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Trade.BusinessServiceDataContract
{
    /// <summary>
    /// This data model is returned by the OPS to the BSL after the order has been processed.
    /// </summary>
    [XmlType(Namespace = "http://trade.samples.websphere.ibm.com", TypeName = "OrderResponseDataBean")]
    [GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
    [DataContract(Namespace = "http://trade.samples.websphere.ibm.com", Name = "OrderResponseDataBean")]
    public class OrderResponseDataModel
    {
        [XmlElement(ElementName = "orderID", Order = 1, IsNullable = false), DataMember(IsRequired = false, Name = "orderID", Order = 1)]
        public string orderID { get; set; }

        [XmlElement(ElementName = "userID", Order = 2, IsNullable = false), DataMember(IsRequired = false, Name = "userID", Order = 2)]
        public string userID { get; set; }

        [XmlElement(ElementName = "price", Order = 3, IsNullable = false), DataMember(IsRequired = false, Name = "price", Order = 3)]
        public decimal price { get; set; }
    }
}
