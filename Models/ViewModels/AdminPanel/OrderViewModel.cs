using System;
using System.Collections.Generic;
using System.Text;
using Models.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.ViewModels.AdminPanel
{
    public class OrderViewModel
    {
       
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public List<ProductOrderView> Products { get; set; }

        public List<NonBarViewModel> ProductNonBar { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public string SellerName { get; set; }
        public double DueAmount { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime SoldAt { get; set; } = DateTime.Now;
    }

    public class ProductOrderView
    {
        public string IndividualProductId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
    }
    public class NonBarViewModel
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public int Amount { get; set; }
    }
}
