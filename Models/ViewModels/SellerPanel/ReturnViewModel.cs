using System.Collections.Generic;

namespace Models.ViewModels.SellerPanel
{
    public class ReturnViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public List<string> Order { get; set; }
        public double TotalPrice { get; set; }
    }
}