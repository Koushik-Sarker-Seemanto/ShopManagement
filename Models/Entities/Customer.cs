using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Entities
{
    public class Customer
    {
        public Customer(string name, string phone)
        {
            CustomerName = name;
            CustomerPhone = phone;
        }
        [BsonId]
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
    }
    

}

