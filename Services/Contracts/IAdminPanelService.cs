using System.Collections.Generic;
using System.Threading.Tasks;
using Models.Entities;
using Models.ViewModels.AdminPanel;

namespace Services.Contracts
{
    public interface IAdminPanelService
    {
        Task<IndexViewModel> GetIndexData();
        Task<EmployeeDetailsViewModel> GetEmployeeDetails(string id);
        public Task<List<Product>> GetAllProducts(string search);
    }
}