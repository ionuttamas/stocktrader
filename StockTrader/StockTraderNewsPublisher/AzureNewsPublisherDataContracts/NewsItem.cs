using System.Runtime.Serialization;

namespace Trade.AzureNewsPublisherDataContracts
{
    [DataContract]
    public class NewsItem
    {
        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
