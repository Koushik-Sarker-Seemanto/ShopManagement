using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.ViewModels.SellerPanel;

namespace Services.Contracts
{
    public interface ISellerPanelService
    {
        public Task<ProductSellViewModel> GetProductFromBar(string id);
        public Task<ProductSellViewModel> GetProductNonBar(string name, string quantity);
        public Task<List<string>> GetProductByName(string query);
        public Task<Order> SellProduct(OrderViewModel model);
        public List<IndividualProduct> GetAllProducts();
    }
}
