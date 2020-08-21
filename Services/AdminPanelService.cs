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
                var dateWiseIncome = await GetDateWiseIncome(DateTime.Now);
                var productSellRepeats = await GetTopSoldProduct(DateTime.Now);
                var topSoldProduct = productSellRepeats.OrderBy(e => e.Repetition).Take(5).ToList();
                var topProfitableProduct = productSellRepeats.OrderBy(e => e.Profit).Take(5).ToList();
                IndexViewModel indexViewModel = new IndexViewModel
                {
                    Managers = managers,
                    Sellers = sellers,
                    DateWiseIncomes = dateWiseIncome,
                    TopSoldProduct = topSoldProduct,
                    TopProfitableProduct = topProfitableProduct,
                };
                return indexViewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetIndexData Failed {ex.Message}");
                return null;
            }
        }

        private async Task<List<ProductSellRepeat>> GetTopSoldProduct(DateTime current)
        {
            var startDay = current.AddDays(-9).Date;
            List<ProductSellRepeat> productSellRepeats = new List<ProductSellRepeat>();

            var products = await this._repository.GetItemsAsync<IndividualProduct>(
                e => e.SellDateTime > startDay && e.SellDateTime < current && e.Sold == true);
            var categories = products.Select(e => e.CategoryId).Distinct();
            foreach (var item in categories)
            {
                var sameTypeProduct = products.Where(e => e.CategoryId == item);
                int repeat = sameTypeProduct.Count();
                double profit = 0;

                foreach (var unitProfit in sameTypeProduct)
                {
                    profit += (unitProfit.SellingPrice - unitProfit.BuyingPrice);
                }
                var product = await this._repository.GetItemAsync<Product>(e => e.Id == item);
                ProductSellRepeat sell = new ProductSellRepeat
                {
                    ProductId = product?.Id,
                    ProductName = product?.Name,
                    Repetition = repeat,
                    Profit = profit,
                };
                productSellRepeats.Add(sell);
            }

            //productSellRepeats = productSellRepeats.OrderBy(e => e.Repetition).Take(5).ToList();
            
            return productSellRepeats;
        }
        
        private async Task<List<DateWiseIncome>> GetDateWiseIncome(DateTime current)
        {
            List<DateWiseIncome> dateWiseIncomes = new List<DateWiseIncome>();
            var startDay = current.AddDays(-9).Date;
            var endDay = current.Date;
            int p = 1;
            _logger.LogInformation($"Date Startttttttttttttttttttttttttttttttttttttttt: ");
            for (var i = startDay; i <= endDay; i = i.AddDays(1))
            {
                _logger.LogInformation($"Date: {p} -> {i}");
                p++;
                FromToDate temp = new FromToDate
                {
                    FromDateTime = i,
                    ToDateTime = i.AddDays(1),
                };
                var result = await this.BusinessStatus(temp);
                DateWiseIncome income = new DateWiseIncome
                {
                    Date = i,
                    DayToDayCalculation = result,
                };
                dateWiseIncomes.Add(income);
            }
            return dateWiseIncomes;
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
        public async Task<List<IndividualProduct>> IndividualProductSoldInDateRange(FromToDate val)
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

        public async Task<Product> ProductById(string id)
        {
            return await _repository.GetItemAsync<Product>(d => d.Id == id);
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

                foreach (var non in order.ProductNonBar)
                {
                    var nonBar = await ProductById(non.ItemId);
                    all[order.SoldAt.Date].TotalBuyingCost += Convert.ToInt32(nonBar.BuyingPrice) * non.Amount;
                }

                all[order.SoldAt.Date].TotalSell += Convert.ToInt32(order.TotalPrice);
                all[order.SoldAt.Date].TotalDue += Convert.ToInt32(order.DueAmount);
                Debug.Print(all[order.SoldAt.Date].TotalSell + " TotAL Sell");
            }

            //Total Buying Price
            var resBuyCost = await IndividualProductSoldInDateRange(val);
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
                res.TotalProfit += entry.Value.TotalProfit;
                res.TotalCost += entry.Value.TotalCost;
                res.TotalDue += entry.Value.TotalDue;
                res.TotalSell += entry.Value.TotalSell;
                res.TotalBuyingCost += entry.Value.TotalBuyingCost;
                Debug.Print(entry.Value.TotalSell + " Total Sell");
            }

            return res;
        }

        public async Task<List<ProductSaleStatus>> ProductSaleStatus(FromToDate val)
        {
            var individualProducts = await IndividualProductSoldInDateRange(val);
            Dictionary<string,ProductSaleStatus> dictionary = new Dictionary<string, ProductSaleStatus>();
            foreach (var individual in individualProducts)
            {
                if (!dictionary.ContainsKey(individual.CategoryId))
                {
                    dictionary[individual.CategoryId] = new ProductSaleStatus();
                    var res = await ProductById(individual.CategoryId);
                    dictionary[individual.CategoryId].ProductName = res.Name;
                    dictionary[individual.CategoryId].CurrentStock = res.Stock;
                    dictionary[individual.CategoryId].ProductPrice = res.SellingPrice;
                }

                dictionary[individual.CategoryId].TotalUnitSale++;
                dictionary[individual.CategoryId].TotalTakaSale += individual.SellingPrice;
                dictionary[individual.CategoryId].TotalProfit += (individual.SellingPrice - individual.BuyingPrice);
                dictionary[individual.CategoryId].AverageBuyingPrice += individual.BuyingPrice;
                dictionary[individual.CategoryId].AverageSalePrice += individual.SellingPrice;
            }

            List<ProductSaleStatus> list = new List<ProductSaleStatus>();
            foreach (KeyValuePair<string, ProductSaleStatus> entry in dictionary)
            {
                entry.Value.AverageBuyingPrice /= entry.Value.TotalUnitSale;
                entry.Value.AverageSalePrice /= entry.Value.TotalUnitSale;
                list.Add(entry.Value);
            }
            


            return list;
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

        public async Task<IndividualProduct> GetIndividualProductById(string id)
        {
            return await _repository.GetItemAsync<IndividualProduct>(d => d.Id == id);
        }

        public async Task<OrderViewModel> GetOrderViewModel(string id)
        {
            var order = await _repository.GetItemAsync<Order>(d => d.Id == id);
            var OrderView = new OrderViewModel();
            OrderView.Id = order.Id;
            OrderView.CustomerName = order.CustomerName;
            OrderView.CustomerPhone = order.CustomerPhone;
            OrderView.SoldAt = order.SoldAt;
            OrderView.TotalPrice = order.TotalPrice;
            OrderView.Discount = order.Discount;
            OrderView.DueAmount = order.DueAmount;
            OrderView.Products = new List<ProductOrderView>();
            foreach (var indi in order.Products)
            {
                var res = await GetIndividualProductById(indi);
                var add = new ProductOrderView();
                add.IndividualProductId = indi;
                add.ProductPrice = res.SellingPrice;
                var product = await ProductById(res.CategoryId);
                add.ProductName = product.Name;
                OrderView.Products.Add(add);
            }
            OrderView.ProductNonBar = new List<NonBarViewModel>();
            foreach (var non in order.ProductNonBar)
            {
                var res = await ProductById(non.ItemId);
                var add = new NonBarViewModel();
                add.Amount = non.Amount;
                add.ItemId = non.ItemId;
                add.ItemName = res.Name;
                OrderView.ProductNonBar.Add(add);
            }

            return OrderView;
        }

        public async Task<bool> PayDue(string orderId, double dueAmount)
        {
            var order = await _repository.GetItemAsync<Order>(d => d.Id == orderId);
            order.DueAmount = dueAmount;
            await _repository.UpdateAsync<Order>(d => d.Id == orderId, order);
            return true;
        }

        public async Task<List<DayToDayCost>> CostStatus(FromToDate date)
        {
            var costs = await CostInDateRange(date);
            var res = new List<DayToDayCost>();
            Dictionary<string,DayToDayCost> dictionary = new Dictionary<string, DayToDayCost>();
            foreach (var cost in costs)
            {
                if (!dictionary.ContainsKey(cost.CreatedAt.Date.ToString()))
                {
                    var daily = new DayToDayCost();
                    dictionary[cost.CreatedAt.Date.ToString()] = daily;
                    dictionary[cost.CreatedAt.Date.ToString()].TotalCost = 0;
                }

                dictionary[cost.CreatedAt.Date.ToString()].date = cost.CreatedAt.Date;
                dictionary[cost.CreatedAt.Date.ToString()].TotalCost += cost.Amount;
            }
            List<DayToDayCost> list = new List<DayToDayCost>();
            foreach (KeyValuePair<string, DayToDayCost> entry in dictionary)
            {
                list.Add(entry.Value);
            }

            return list;
        }
    }
}