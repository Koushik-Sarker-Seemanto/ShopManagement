using System;
using System.Collections.Generic;
using System.Text;
using Models.Entities;

namespace Models.ViewModels.SellerPanel
{
    public class ProductDetails
    {
        public IndividualProduct Unit { get; set; }
        public Product Product { get; set; }
        public Order Order { get; set; }
    }
}
