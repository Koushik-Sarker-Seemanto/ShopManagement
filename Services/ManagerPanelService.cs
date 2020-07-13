using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.ManagerPanelModels;
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
        public async Task<Product> AddProduct(ProductViewModel model)
        {
            try
            {
                if (model != null)
                {
                    var guid = Guid.NewGuid().ToString();
                    var product = new Product
                    {
                        Id = guid,
                        Name = model.Name,
                        BuyingPrice = model.BuyingPrice,
                        SellingPrice = model.SellingPrice,
                        Details = model.Details,
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
    }
}