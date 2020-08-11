using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class Order
    {
        [BsonId]
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        [BsonRequired]
        public List<string> Products { get; set; }

        public List<NonBar> ProductNonBar { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public double DueAmount { get; set; }
    }

    public class NonBar
    {
        public string ItemId { get; set; }
        public int Amount { get; set; }
    }
}