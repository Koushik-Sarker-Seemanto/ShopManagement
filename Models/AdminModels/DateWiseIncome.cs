using System;
using Models.ViewModels.AdminPanel;

namespace Models.AdminModels
{
    public class DateWiseIncome
    {
        public DateTime Date { get; set; }
        public DayToDayCalculation DayToDayCalculation { get; set; }
    }
}