using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class IndividualProduct
    {
        [BsonId]
        public string Id { get; set; }

        public string CategoryId { get; set; }
        public double BuyingPrice { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Sold { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime SellDateTime { get; set; }
    }
}