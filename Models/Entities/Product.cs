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

        public string ProductType { get; set; }
        
        public int Stock { get; set; } = 0;
        public int StockWarning { get; set; } = 0;
        [BsonDefaultValue(true)]
        public bool Warning { get; set; }
        public string Details { get; set; }
    }
}