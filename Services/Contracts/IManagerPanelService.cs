using System.Collections.Generic;
using System.Drawing;
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
        public  Task<Product> FindProductById(string modelId);
        public Task<Product> UpdateCurrentStock(string productId, int stockAmount, double buyingPrice);


    }
}