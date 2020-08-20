using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.SellerPanelModels;
using Models.ViewModels.SellerPanel;

namespace Services.Contracts
{
    public interface ISellerPanelService
    {
        public Task<ProductSellViewModel> GetProductFromBar(string id, bool returnProduct);
        public Task<ProductSellViewModel> GetProductNonBar(string name, string quantity);
        public Task<List<ProductSuggestion>> GetProductByName(string query);
        public Task<Order> SellProduct(OrderViewModel model);
        public Task<ReturnProduct> ReturnProduct(ReturnViewModel model);
        public List<IndividualProduct> GetAllProducts();
        public  Task<ProductDetails> GetAllDetail(ProductIdInput product);
    }
}
