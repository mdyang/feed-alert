namespace feed_alert.Entity
{
    using System.Runtime.Serialization;

    [DataContractAttribute]
    class FeedSource
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
