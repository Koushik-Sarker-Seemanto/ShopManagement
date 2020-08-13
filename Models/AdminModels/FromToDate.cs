using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Models.AdminModels
{
    public class FromToDate
    {
        [Required]
        public DateTime FromDateTime { get; set; }
        [Required]
        public DateTime ToDateTime { get; set; }
    }
}
