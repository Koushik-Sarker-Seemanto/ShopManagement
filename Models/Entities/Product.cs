using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    [Serializable]
    public class Product
    {
        [BsonId]
        public string Id { get; set; }
        [BsonRequired]
        public string Name { get; set; }
        [BsonRequired]
        public double SellingPrice { get; set; }
        [BsonRequired]
        public double BuyingPrice { get; set; }
        public string Details { get; set; }
    }
}