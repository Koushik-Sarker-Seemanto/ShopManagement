using System;
using System.Collections.Generic;
using System.Text;
using Models.Entities;

namespace Models.ViewModels.SellerPanel
{
    public class OrderViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<string> Order { get; set; }
        public List<NonBar> OrderNonBar { get; set; }
        public double TotalPrice { get; set; }
        public double Discount { get; set; }
        public double DueAmount { get; set; }
    }
}
