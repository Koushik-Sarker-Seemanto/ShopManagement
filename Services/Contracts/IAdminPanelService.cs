using System.Threading.Tasks;
using Models.ViewModels.AdminPanel;

namespace Services.Contracts
{
    public interface IAdminPanelService
    {
        Task<IndexViewModel> GetIndexData();
        Task<EmployeeDetailsViewModel> GetEmployeeDetails(string id);
    }
}