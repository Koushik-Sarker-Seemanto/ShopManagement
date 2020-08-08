using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.SellerPanel
{
    public class OrderViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<string> Order { get; set; }
        public bool Paid { get; set; }
        public int Amount { get; set; }
        public int Discount { get; set; }
    }
}
