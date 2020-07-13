using System.ComponentModel.DataAnnotations;

namespace Models.ManagerPanelModels
{
    public class ProductViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public double SellingPrice { get; set; }
        [Required]
        public double BuyingPrice { get; set; }
        public string Details { get; set; }
    }
}