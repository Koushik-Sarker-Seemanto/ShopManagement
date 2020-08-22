using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.AdminPanel
{
    public class StockAmount
    {
        public List<ProductView> ProductList { get; set; }
        public double TotalAmount { get; set; }
    }
    public class ProductView
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public double StockValue { get; set; }
        public double AverageUnitprice { get; set; }
    }
}
