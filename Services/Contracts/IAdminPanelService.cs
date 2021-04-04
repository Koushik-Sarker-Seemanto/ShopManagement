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
	Task<List<OrderViewModel>> GetAllOrderViewModels(FromToDate dateRange);
        Task<EmployeeDetailsViewModel> GetEmployeeDetails(string id);
        public Task<List<Product>> GetAllProducts(string search);
        public IQueryable<OrderTable> GetAllDueOrders();
        public IQueryable<OrderTable> GetAllOrders(FromToDate val);
        public Task<DayToDayCalculation> BusinessStatus(FromToDate val);
        Task<List<ProductSaleStatus>> ProductSaleStatus(FromToDate val);
        Task<OrderViewModel> GetOrderViewModel(string id);
        Task<bool> PayDue(string orderId, double dueAmount);
        Task<List<DayToDayCost>> CostStatus(FromToDate date);
        Task<StockAmount> GetFullStockAmount();
    }
}
