using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.SellerPanel
{
    public class ProductSellViewModel
    {
        public string Id { get; set; }
        public string ProductTitle { get; set; }
        public double SellingPrice { get; set; }
        public bool Sold { get; set; }
    }
}
