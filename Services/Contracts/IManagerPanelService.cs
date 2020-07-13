using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Entities;
using Models.ManagerPanelModels;

namespace Services.Contracts
{
    public interface IManagerPanelService
    {
        Task<Product> AddProduct(ProductViewModel model);
        IQueryable<Product> GetAllProducts();
    }
}