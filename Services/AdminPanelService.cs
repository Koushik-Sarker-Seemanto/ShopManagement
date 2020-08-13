using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Models.AdminModels;
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

        public async Task<EmployeeDetailsViewModel> GetEmployeeDetails(string id)
        {
            try
            {
                var employee = await _repository.GetItemAsync<User>(e => e.Id == id);
                if (employee != null)
                {
                    employee.Password = null;
                }
                EmployeeDetailsViewModel viewModel = new EmployeeDetailsViewModel
                {
                    Employee = employee,
                };
                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetEmployeeDetails Failed {ex.Message}");
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

        public async Task<List<Product>> GetAllProducts(string search)
        {
            var items = await _repository.GetItemsAsync<Product>(d => d.Name.Contains(search));
            return items.ToList();
        }
        public IQueryable<Order> GetAllDueOrders()
        {
            var items =  _repository.GetItems<Order>(d=>d.DueAmount > 0.00);
       
            return items;
        }

        public async Task<List<Order>> OrderInDateRange(FromToDate val)
        {
            var ress = await _repository.GetItemsAsync<Order>(d => d.SoldAt > val.FromDateTime && d.SoldAt < val.ToDateTime);
            var res = ress?.ToList();
            Debug.Print(res.Count + " " + val.FromDateTime.ToString()+" "+ val.ToDateTime.ToString());
            return res;
        }
        public async Task<List<IndividualProduct>> IndividualProductInDateRange(FromToDate val)
        {
            var ress = await _repository.GetItemsAsync<IndividualProduct>(d => d.SellDateTime > val.FromDateTime && d.SellDateTime < val.ToDateTime && d.Sold == true);
            var res = ress?.ToList();
            // Debug.Print(res.Count + " " + val.FromDateTime.ToString() + " " + val.ToDateTime.ToString());
            return res;
        }

        public async Task<List<Cost>> CostInDateRange(FromToDate val)
        {
            var ress = await _repository.GetItemsAsync<Cost>(d => d.CreatedAt > val.FromDateTime && d.CreatedAt < val.ToDateTime );
            var res = ress?.ToList();
            // Debug.Print(res.Count + " " + val.FromDateTime.ToString() + " " + val.ToDateTime.ToString());
            return res;
        }


        public async Task<DayToDayCalculation> BusinessStatus(FromToDate val)
        {
            var resOrder = await OrderInDateRange(val);
            Dictionary<DateTime,DailyCalculation> all = new Dictionary<DateTime, DailyCalculation>();
            
            // Total Sell  &&&
            // Total Due
            foreach (var order in resOrder)
            {
                
                if (!all.ContainsKey(order.SoldAt.Date))
                {
                    all[order.SoldAt.Date] = new DailyCalculation(order.SoldAt.Date);
                }

                all[order.SoldAt.Date].TotalSell += Convert.ToInt32(order.TotalPrice);
                all[order.SoldAt.Date].TotalDue += Convert.ToInt32(order.DueAmount);
                Debug.Print(all[order.SoldAt.Date].TotalSell + " TotAL Sell");
            }

            //Total Buying Price
            var resBuyCost = await IndividualProductInDateRange(val);
            foreach (var product in resBuyCost)
            {
                if (!all.ContainsKey(product.SellDateTime.Date))
                {
                    all[product.SellDateTime.Date] = new DailyCalculation(product.SellDateTime.Date);
                }

                all[product.SellDateTime.Date].TotalBuyingCost += Convert.ToInt32(product.BuyingPrice);
            }

            var resCost = await CostInDateRange(val);
            foreach (var cost in resCost)
            {
                if(!all.ContainsKey(cost.CreatedAt.Date))
                {
                    all[cost.CreatedAt.Date] = new DailyCalculation(cost.CreatedAt.Date);
                }

                all[cost.CreatedAt.Date].TotalCost += Convert.ToInt32(cost.Amount);
            }
            var res = new DayToDayCalculation();
            res.dailyCalculations = new List<DailyCalculation>();
            foreach (KeyValuePair<DateTime, DailyCalculation> entry in all)
            {
                entry.Value.TotalProfit = entry.Value.TotalSell - entry.Value.TotalCost - entry.Value.TotalBuyingCost; 
                res.dailyCalculations.Add( entry.Value);
                Debug.Print(entry.Value.TotalSell + " Total Sell");
            }

            return res;
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