using System.Collections.Generic;
using Models.AdminModels;
using Models.Entities;

namespace Models.ViewModels.AdminPanel
{
    public class IndexViewModel
    {
        public List<User> Managers { get; set; }
        public List<User> Sellers { get; set; }
        public List<DateWiseIncome> DateWiseIncomes { get; set; }
        public List<ProductSellRepeat> TopSoldProduct { get; set; }
    }
}