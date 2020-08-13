using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.AdminPanel
{
    public class DayToDayCalculation
    {
        public List<DailyCalculation> dailyCalculations { get; set; }
    }

    public class DailyCalculation
    {
        public DailyCalculation(DateTime days)
        {
            day = days;
            TotalBuyingCost = 0;
            TotalCost = 0;
            TotalDue = 0;
            TotalSell = 0;
            TotalProfit = 0;
        }
        public DateTime day { get; set; }
        public int TotalSell { get; set; }
        public int TotalBuyingCost { get; set; }
        public int TotalCost { get; set; }
        public int TotalProfit { get; set; }
        public int TotalDue { get; set; }
    }
}
