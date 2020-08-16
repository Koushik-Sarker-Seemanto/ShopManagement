using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.AdminPanel
{
    public class ProductSaleStatus
    {
        public ProductSaleStatus()
        {
            ProductPrice = 0;
            CurrentStock = 0;
            TotalProfit = 0;
            TotalUnitSale = 0;
            TotalTakaSale = 0;
            AverageBuyingPrice = 0;
            AverageSalePrice = 0;
        }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public int CurrentStock { get; set; }
        public double TotalUnitSale { get; set; }
        public double TotalTakaSale { get; set; }
        public double TotalProfit { get; set; }
        public double AverageSalePrice { get; set; }
        public double AverageBuyingPrice { get; set; }
    }
}
