using System.Threading.Tasks;
using Models.Entities;
using Models.ManagerPanelModels;

namespace Services.Contracts
{
    public interface IManagerPanelService
    {
        Task<Product> AddProduct(ProductViewModel model);
    }
}