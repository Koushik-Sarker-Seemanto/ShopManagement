using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;
using Models.ManagerPanelModels;
using Models.ViewModels.AdminPanel;
using Models.ViewModels.SellerPanel;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class SellerPanelService : ISellerPanelService
    {
        private readonly IMongoRepository _repository;
        public SellerPanelService(IMongoRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductSellViewModel> GetProductFromBar(string id)
        {
            ProductSellViewModel resProduct = null;
            if (id != null)
            {
                var indevidualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id);
                if (indevidualProduct != null)
                {
                    var product = await _repository.GetItemAsync<Product>(d => d.Id == indevidualProduct.CategoryId);
                    if (product != null)
                    {
                        resProduct = new ProductSellViewModel();
                        resProduct.Id = indevidualProduct.Id;
                        resProduct.ProductTitle = product.Name;
                        resProduct.SellingPrice = product.SellingPrice;
                        return resProduct;
                    }
                }
            }

            return null;
        }
    }
}
