using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class Order
    {
        [BsonId]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<IndividualProduct> Products { get; set; }
        [BsonDefaultValue(false)]
        public bool Paid { get; set; }
        public int Amount { get; set; }
        [BsonDefaultValue(0)]
        public int Discount { get; set; }
    }
}
