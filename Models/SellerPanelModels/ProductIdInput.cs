using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.SellerPanelModels
{
    public class ProductIdInput
    {
        [Required]
        public string ProductId { get; set; }
    }
}
