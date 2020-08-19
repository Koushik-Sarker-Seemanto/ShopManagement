using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.ManagerPanelModels;
using Models.SellerPanelModels;
using Models.ViewModels.AdminPanel;
using Models.ViewModels.SellerPanel;
using Repositories;
using Services.Contracts;
using OrderViewModel = Models.ViewModels.SellerPanel.OrderViewModel;

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

        public async Task<ProductSellViewModel> GetProductFromBar(string id, bool returnProduct)
        {
            ProductSellViewModel resProduct = null;
            if (id != null)
            {
                IndividualProduct indevidualProduct = null;
                if (!returnProduct)
                {
                    indevidualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id && d.Sold == false);
                }
                else
                {
                    indevidualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id && d.Sold == true);
                }
                if (indevidualProduct != null)
                {
                    var product = await _repository.GetItemAsync<Product>(d => d.Id == indevidualProduct.CategoryId);
                    if (product != null)
                    {
                        resProduct = new ProductSellViewModel
                        {
                            Id = indevidualProduct.Id,
                            ProductTitle = product.Name,
                        };
                        if (returnProduct)
                        {
                            resProduct.SellingPrice = indevidualProduct.SellingPrice;
                        }

                        resProduct.SellingPrice = product.SellingPrice;
                        return resProduct;
                    }
                }
            }

            return null;
        }

        public async Task<ProductSellViewModel> GetProductNonBar(string name, string quantity)
        {
            try
            {
                var result = await _repository.GetItemAsync<Product>(e => e.Name == name && e.ProductType == "NonBarcode");
                if (result != null)
                {
                    if (result.Stock < Double.Parse(quantity))
                    {
                        return null;
                    }
                    ProductSellViewModel response = new ProductSellViewModel()
                    {
                        Id = result.Id,
                        ProductTitle = result.Name,
                        SellingPrice = result.SellingPrice,
                    };
                    return response;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetProductNonBar Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<List<string>> GetProductByName(string query)
        {
            try
            {
                var result = _repository.GetItems<Product>(e => e.Name.ToUpper().Contains(query.ToUpper()) && e.ProductType == "NonBarcode");
                return result?.Select(e => e.Name).ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"GetProductByName Failed: {ex.Message}");
                return null;
            }
        }

        

        public async Task<Order> SellProduct(OrderViewModel model)
        {
            try
            {
                var orderId = Guid.NewGuid().ToString();
                if (model?.OrderNonBar != null)
                {
                    foreach (var item in model.OrderNonBar)
                    {
                        var product = await _repository.GetItemAsync<Product>(e => e.Id.Equals(item.ItemId));
                        if (product != null)
                        {
                            product.Stock = product.Stock - item.Amount;
                            
                            await _repository.UpdateAsync<Product>(
                                e => e.Id == product.Id, product);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                if (model?.Order != null)
                {
                    foreach (var item in model.Order)
                    {
                        var individualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == item && d.Sold == false);
                        if (individualProduct != null)
                        {
                            var product =
                                await _repository.GetItemAsync<Product>(e => e.Id.Equals(individualProduct.CategoryId));
                            if (product != null)
                            {
                                product.Stock = product.Stock - 1;
                                await _repository.UpdateAsync<Product>(
                                    e => e.Id == product.Id, product);
                            }

                            individualProduct.SellingPrice = product.SellingPrice;

                            individualProduct.OrderId = orderId;
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
                }

                if (model?.Order != null || model?.OrderNonBar != null)
                {
                    
                    Order order = new Order
                    {
                        Id = orderId,
                        CustomerName = model.Name,
                        CustomerPhone = model.Phone,
                        Products = model.Order,
                        ProductNonBar = model.OrderNonBar,
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

        public async Task<ReturnProduct> ReturnProduct(ReturnViewModel model)
        {
            try
            {
                if (model?.Order != null)
                {
                    var id = Guid.NewGuid().ToString();
                    foreach (var item in model.Order)
                    {
                        var individualProduct = await _repository.GetItemAsync<IndividualProduct>(d => d.Id == item && d.Sold == true);
                        if (individualProduct != null)
                        {
                            var product =
                                await _repository.GetItemAsync<Product>(e => e.Id.Equals(individualProduct.CategoryId));
                            if (product != null)
                            {
                                product.Stock = product.Stock + 1;
                                await _repository.UpdateAsync<Product>(
                                    e => e.Id == product.Id, product);
                            }
                            
                            var order = await _repository.GetItemAsync<Order>(e => e.Id == individualProduct.OrderId);
                            if (order != null)
                            {
                                var temp = order.Products;
                                temp.Remove(individualProduct.Id);
                                order.Products = temp;
                                order.TotalPrice = order.TotalPrice - individualProduct.SellingPrice;
                                
                                await _repository.UpdateAsync<Order>(e => e.Id == order.Id, order);
                            }
                            
                            individualProduct.Sold = false;
                            if (individualProduct.ReturnIdList == null)
                            {
                                individualProduct.ReturnIdList = new List<string>();
                            }
                            individualProduct.ReturnIdList.Add(id);
                            individualProduct.OrderId = String.Empty;
                            await _repository.UpdateAsync<IndividualProduct>(
                                e => e.Id == individualProduct.Id, individualProduct);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    ReturnProduct returnProduct = new ReturnProduct
                    {
                        Id = id,
                        Products = model.Order,
                        CustomerName = model.Name,
                        CustomerPhone = model.Phone,
                        ReturnAt = DateTime.Now,
                        TotalPrice = model.TotalPrice,
                    };
                    await _repository.SaveAsync<ReturnProduct>(returnProduct);
                    return returnProduct;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"ReturnProduct Failed: {ex.Message}");
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

       

        public async Task<ReturnProduct> GetReturnProduct(string id)
        {
            return await _repository.GetItemAsync<ReturnProduct>(d => d.Id == id);
        }
        public async Task<IndividualProduct> GetIndividualProductById(string id)
        {
            return await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id);
        }
        public async Task<Product> GetProductById(string id)
        {
            return await _repository.GetItemAsync<Product>(d => d.Id == id);
        }
        public async Task<Order> GetOrderById(string id)
        {
            return await _repository.GetItemAsync<Order>(d => d.Id == id);
        }

        public async Task<ProductDetails> ProductDetailWithReturn(ProductIdInput product)
        {
           
                var indi = await GetIndividualProductById(product.ProductId);
                var res = new ProductDetails();
                res.Unit = buildIndividualProductView(indi);
                res.Product = await GetProductById(indi.CategoryId);
                if (indi.Sold)
                {
                    res.Order = await GetOrderById(indi.OrderId);
                }

                if (indi.ReturnIdList != null)
                {
                    foreach (var returnL in indi.ReturnIdList)
                    {
                        var ret = await GetReturnProduct(returnL);
                        var retView = new ReturnUnitView();
                        retView.Id = ret.Id;
                        retView.CustomerName = ret.CustomerPhone;
                        retView.CustomerPhone = ret.CustomerPhone;
                        retView.ReturnTime = ret.ReturnAt;
                        res.Unit.ReturnIdList.Add(retView);
                    }
                }

                return res;


            
        }

        private IndividualProductView buildIndividualProductView(IndividualProduct product)
        {
            var res = new IndividualProductView();
            res.Id = product.Id;
            res.CategoryId = product.CategoryId;
            res.CreatedAt = product.CreatedAt;
            res.BuyingPrice = product.BuyingPrice;
            res.SellDateTime = product.SellDateTime;
            res.OrderId = product.OrderId;
            res.Sold = product.Sold;
            res.SellingPrice = product.SellingPrice;
            res.ReturnIdList = new List<ReturnUnitView>();
            return res;
        }
    }
}
