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
        Task<Product> EditProduct(ProductViewModel model);
        IQueryable<Product> GetAllProducts();
        public  Task<Product> FindProductById(string modelId);
        Task<ProductViewModel> FindProductViewModelById(string modelId);
        public Task<List<string>> UpdateCurrentStock(string productId, int stockAmount, double buyingPrice);
        public  Task<Product> StockReduce(string id);
        public IQueryable<Product> GetAllStockEnd();
        public Task<Cost> AddCost(Cost cost);


    }
}