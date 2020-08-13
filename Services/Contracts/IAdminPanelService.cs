using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.AdminModels;
using Models.Entities;
using Models.ViewModels.AdminPanel;

namespace Services.Contracts
{
    public interface IAdminPanelService
    {
        Task<IndexViewModel> GetIndexData();
        Task<EmployeeDetailsViewModel> GetEmployeeDetails(string id);
        public Task<List<Product>> GetAllProducts(string search);
        public IQueryable<Order> GetAllDueOrders();
        public Task<DayToDayCalculation> BusinessStatus(FromToDate val);
    }
}