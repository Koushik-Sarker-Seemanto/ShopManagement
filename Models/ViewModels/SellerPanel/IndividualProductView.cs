using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.SellerPanel
{
    public class IndividualProductView
    {
       
        public string Id { get; set; }

        public string CategoryId { get; set; }
        public double BuyingPrice { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool Sold { get; set; }
        public double SellingPrice { get; set; }
        public string OrderId { get; set; }
        public List<ReturnUnitView> ReturnIdList { get; set; }
        
        public DateTime SellDateTime { get; set; }
    }
    public class ReturnUnitView {
        public string Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public DateTime ReturnTime { get; set; }

    }
}
