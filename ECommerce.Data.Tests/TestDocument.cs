using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECommerce.Data.Tests
{
    [DataContract]
    public class TestDocument
    {
        [BsonId, IgnoreDataMember]
        public ObjectId _id { get; set; }

        [Key, DataMember]
        public string Key { get; set; }
        [DataMember]
        public string ApplicationName { get; set; }
        [DataMember]
        public string Value { get; set; }
    }
}