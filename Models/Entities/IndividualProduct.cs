using System;
using System.Collections.Generic;
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
        public double SellingPrice { get; set; }
        public string OrderId { get; set; }
        public List<string> ReturnIdList { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime SellDateTime { get; set; }
    }
}