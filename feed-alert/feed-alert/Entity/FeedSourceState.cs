namespace feed_alert.Entity
{
    using System.Runtime.Serialization;

    [DataContractAttribute]
    class FeedSourceState
    {
        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public string LastEntryUrl { get; set; }

        [DataMember]
        public string LastModifiedDate { get; set; }
    }
}
