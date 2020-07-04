using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.CommonEnums;
using Models.Entities;
using Models.ViewModels.AdminPanel;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class AdminPanelService : IAdminPanelService
    {
        private ILogger<IAdminPanelService> _logger;
        private IMongoRepository _repository;
        public AdminPanelService(ILogger<IAdminPanelService> logger, IMongoRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }
        public async Task<IndexViewModel> GetIndexData()
        {
            try
            {
                var managers = await GetAllManager();
                var sellers = await GetAllSeller();
                IndexViewModel indexViewModel = new IndexViewModel
                {
                    Managers = managers,
                    Sellers = sellers,
                };
                return indexViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetIndexData Failed {ex.Message}");
                return null;
            }
        }
        
        private async Task<List<User>> GetAllManager()
        {
            try
            {
                var managers = await _repository.GetItemsAsync<User>(e => e.Role == Roles.Manager);
                managers = managers?.Select(e => e ).OrderBy(e => e.CreatedAt);
                List<User> allManagers = new List<User>();
                if (managers != null)
                {
                    foreach (var user in managers)
                    {
                        user.Password = null;
                        allManagers.Add(user);
                    }
                }
                return allManagers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllManager Failed {ex.Message}");
                return null;
            }
        }

        private async Task<List<User>> GetAllSeller()
        {
            try
            {
                var sellers = await _repository.GetItemsAsync<User>(e => e.Role == Roles.Seller);
                sellers = sellers?.Select(e => e ).OrderBy(e => e.CreatedAt);
                List<User> allSellers = new List<User>();
                if (sellers != null)
                {
                    foreach (var user in sellers)
                    {
                        user.Password = null;
                        allSellers.Add(user);
                    }
                }
                return allSellers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllSeller Failed {ex.Message}");
                return null;
            }
        }
    }
}