using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IManagerPanelService _managerPanelService;
        private readonly IMongoRepository _repository;
        public SellerPanelService(IManagerPanelService managerPanelService,IMongoRepository repository)
        {
            _managerPanelService = managerPanelService;
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

                    var Mainproduct = await _managerPanelService.FindProductById(indevidualProduct.CategoryId);
                    if (Mainproduct != null)
                    {
                        if (Mainproduct.Stock <= 0)
                        {
                            return null;
                        }
                    }
                    if (indevidualProduct != null)
                    {
                        var product = await _repository.GetItemAsync<Product>(d => d.Id == indevidualProduct.CategoryId);
                        if (product != null)
                        {
                            resProduct = new ProductSellViewModel();
                            resProduct.Id = indevidualProduct.Id;
                            resProduct.ProductTitle = product.Name;
                            resProduct.SellingPrice = product.SellingPrice;
                            resProduct.Sold = false;
                            
                            Debug.Print(indevidualProduct.SoldAt.Year + "Day OF YEAR");
                            if (indevidualProduct.SoldAt != null)
                            {
                                if (indevidualProduct.SoldAt.Year > 2000)
                                {
                                    resProduct.Sold = true;
                                }
                            }


                            return resProduct;
                        }
                    }
                }
            }

            return null;
        }


        public async Task<Order> MakeOrder(OrderViewModel order)
        {
            var id = Guid.NewGuid().ToString();
            var orderModel = new Order
            {
                Id = id,
                Name = order.Name,
                Phone = order.Phone,
                Amount = order.Amount,
                Paid    = order.Paid,
                Discount = order.Discount
            };
            
            var list = await GetIndividualProducts(order.Order);
            if (list == null) return null;
            orderModel.Products = list;
            await _repository.SaveAsync<Order>(orderModel);
            await UpdateIndividualProduct(orderModel.Products);
            return orderModel;
        }

        public async Task<List<IndividualProduct>> GetIndividualProducts(List<string> ids)
        {
            var list = new List<IndividualProduct>();
            foreach (var id in ids)
            {
                Debug.Print(id);
                var indiPro = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id);
                if (indiPro != null)
                {
                    list.Add(indiPro);
                }
                else
                {
                    return null;
                }
            }
            return list;
        }

        public async Task UpdateIndividualProduct(List<IndividualProduct> product)
        {
            if (product != null)
            {
                foreach (var singleProduct in product)
                {
                    if (singleProduct != null)
                    {
                        singleProduct.Sold = true;
                        singleProduct.SoldAt = DateTime.Now;
                        await _managerPanelService.StockReduce(singleProduct.CategoryId);
                        await _repository.UpdateAsync<IndividualProduct>(d => d.Id == singleProduct.Id, singleProduct);

                    }
                }

            }
        }

    }
}
