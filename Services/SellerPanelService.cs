using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<SellerPanelService> logger;
        public SellerPanelService(IMongoRepository repository, ILogger<SellerPanelService> logger)
        {
            _repository = repository;
            this.logger = logger;
        }

        public async Task<ProductSellViewModel> GetProductFromBar(string id)
        {
            ProductSellViewModel resProduct = null;
            if (id != null)
            {
                var indevidualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id && d.Sold == false);
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

        public async Task<Order> SellProduct(OrderViewModel model)
        {
            try
            {
                if (model?.Order != null && model.Order.Count > 0)
                {
                    foreach (var item in model.Order)
                    {
                        var individualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == item && d.Sold == false);
                        if (individualProduct != null)
                        {
                            individualProduct.Sold = true;
                            individualProduct.SellDateTime = DateTime.Now;
                            await _repository.UpdateAsync<IndividualProduct>(
                                e => e.Id == individualProduct.Id, individualProduct);
                        }
                        else
                        {
                            return null;
                        }
                    }

                    var orderId = Guid.NewGuid().ToString();
                    Order order = new Order
                    {
                        Id = orderId,
                        CustomerName = model.Name,
                        CustomerPhone = model.Phone,
                        Products = model.Order,
                        Discount = model.Discount,
                        DueAmount = model.DueAmount,
                        TotalPrice = model.TotalPrice,
                    };
                    await _repository.SaveAsync<Order>(order);
                    return order;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"SellProduct Failed: {ex.Message}");
                return null;
            }
        }

        public List<IndividualProduct> GetAllProducts()
        {
            try
            {
                var result =  _repository.GetItems<IndividualProduct>().ToList();
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetAllProduces Failed: {ex.Message}");
                return null;
            }
        }
    }
}
