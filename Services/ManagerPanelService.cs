using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aspose.BarCode.Generation;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.ManagerPanelModels;
using MongoDB.Bson;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class ManagerPanelService: IManagerPanelService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<IManagerPanelService> _logger;
        public ManagerPanelService(IMongoRepository repository, ILogger<IManagerPanelService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        static object locker = new object();
        static string Generate15UniqueDigits()
        {
            lock (locker)
            {
                Thread.Sleep(100);
                return DateTime.Now.ToString("yyyyMMddHHmmssf");
            }
        }
        public async Task<Product> AddProduct(ProductViewModel model)
        {
            try
            {
                if (model != null)
                {
                    var exist = await _repository.GetItemAsync<Product>(d => d.Name == model.Name);
                    if (exist != null)
                    {
                        return null;
                    }
                    var guid = Guid.NewGuid().ToString();
                    var product = new Product
                    {
                        Id = guid,
                        Name = model.Name,
                        StockWarning = model.StockWarning,
                        SellingPrice = model.SellingPrice,
                        Details = model.Details,
                        BuyingPrice = model.BuyingPrice,
                        ProductType = model.ProductType,
                    };
                    await _repository.SaveAsync<Product>(product);
                    return product;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddProduct Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Product> EditProduct(ProductViewModel model)
        {
            try
            {
                if (model != null)
                {
                    
                    // var guid = Guid.NewGuid().ToString();
                    var product = new Product
                    {
                        Id = model.Id,
                        Name = model.Name,
                        StockWarning = model.StockWarning,
                        SellingPrice = model.SellingPrice,
                        BuyingPrice = model.BuyingPrice,
                        Details = model.Details,
                        ProductType = model.ProductType,
                    };
                    await _repository.UpdateAsync<Product>(d=>d.Id == model.Id,product);
                    return product;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddProduct Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Product> FindProductById(string modelId)
        {
            try
            {
                if (modelId != null)
                {
                    var pro =  await _repository.GetItemAsync<Product>(d => d.Id == modelId);
                    return pro;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FindProductById Failed: {ex.Message}");
                return null;
            }
        }
        public async Task<ProductViewModel> FindProductViewModelById(string modelId)
        {
            try
            {
                if (modelId != null)
                {
                    var pro = await _repository.GetItemAsync<Product>(d => d.Id == modelId);
                    var res = new ProductViewModel();
                    res.Id = pro.Id;
                    res.Name = pro.Name;
                    res.SellingPrice = pro.SellingPrice;
                    res.StockWarning = pro.StockWarning;
                    res.Details = pro.Details;
                    res.BuyingPrice = pro.BuyingPrice;
                    res.ProductType = pro.ProductType;
                    Debug.Print(res.Name + "aascccccccccccccccccccccc");
                    return res;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"FindProductById Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Product> StockReduce(string id)
        {
            var product = await FindProductById(id);
            product.Stock -= 1;
            if (product.StockWarning >= product.Stock) product.Warning = true;
            await _repository.UpdateAsync<Product>(d => d.Id == id, product);
            return product;
        }

        public async Task<List<string>> UpdateCurrentStock(string productId, int stockAmount, double buyingPrice)
        {
            try
            {
                var product = await FindProductById(productId);
                if (product == null)
                {
                    return null;
                }
                product.Stock += stockAmount;
                if (product.StockWarning < product.Stock) product.Warning = false;
                await _repository.UpdateAsync<Product>(d => d.Id == productId, product);
                
                if (product.ProductType == "Barcode")
                {
                    List<string> productList = new List<string>();
                    for (int i = 1; i <= stockAmount; i++)
                    {
                        
                        var uni = Generate15UniqueDigits();
                        var id = uni;
                        var individualProduct = new IndividualProduct
                        {
                            Id = id,
                            CategoryId = productId,
                            BuyingPrice = buyingPrice,
                        };
                        productList.Add(id);
                        await _repository.SaveAsync<IndividualProduct>(individualProduct);
                    }
                    _logger.LogInformation($"----------------Calling GenerateBarcode-------------------");
                    return productList;
                    //GenerateBarCode(productList,product.Name);
                }
                
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"UpdateCurrentStock: {e.Message}");
                return null;
            }
        }


        
        public IQueryable<Product> GetAllProducts()
        {
            try
            {
                var results = _repository.GetItems<Product>();
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllProducts Failed: {ex.Message}");
                return null;
            }
        }
        public IQueryable<Product> GetAllStockEnd()
        {
            try
            {
                var response = new List<Product>();
                var results = _repository.GetItems<Product>();
                foreach (var item in results)
                {
                    if (item.StockWarning >= item.Stock)
                    {
                        response.Add(item);
                    }
                }
                
                return response.AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllProducts Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Cost> AddCost(Cost cost)
        {
            cost.Id = Guid.NewGuid().ToString();
            await _repository.SaveAsync<Cost>(cost);
            return cost;
        }

    }
}