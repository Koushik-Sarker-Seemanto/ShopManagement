using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ViewModels.AdminPanel
{
    public class DailyCostStatus
    {
        public List<DayToDayCost> DailyCosts { get; set; }
    }
    public class DayToDayCost
    {
        public DateTime date { get; set; }
        public int TotalCost { get; set; }
    }
}
