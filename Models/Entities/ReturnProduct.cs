using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class ReturnProduct
    {
        [BsonId]
        public string Id { get; set; }
        [BsonRequired]
        public string CustomerName { get; set; }
        [BsonRequired]
        public string CustomerPhone { get; set; }
        [BsonRequired]
        public List<string> Products { get; set; }
        public double TotalPrice { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ReturnAt { get; set; } = DateTime.Now;
    }
}