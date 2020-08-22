using System;
using System.Collections.Generic;
using System.Text;
using Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.ViewModels.AdminPanel
{
    public class OrderTable
    {
        
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public double DueAmount { get; set; }
        public string SellerName { get; set; }
        public string SoldAt { get; set; } 
    }
}
