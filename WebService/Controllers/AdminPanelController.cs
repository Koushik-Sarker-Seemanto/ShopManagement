using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
using Models.AdminModels;
using Models.Entities;
using Models.ViewModels.AdminPanel;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminPanelController : Controller
    {
        private IUserServices _userServices;
        private ILogger<AdminPanelController> logger;
        private IAdminPanelService _adminPanelService;
        public AdminPanelController(ILogger<AdminPanelController> logger, IUserServices userServices, IAdminPanelService adminPanelService)
        {
            this.logger = logger;
            _userServices = userServices;
            _adminPanelService = adminPanelService;
        }
        // GET
        public async Task<IActionResult> Index()
        {
            var indexViewModel = await _adminPanelService.GetIndexData();
            return View(indexViewModel);
        }

        public async Task<IActionResult> EmployeeDetails(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");
            }
            var viewModel = await _adminPanelService.GetEmployeeDetails(id);
            return View(viewModel);
        }

        public IActionResult AddEmployee()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee([Bind] RegisterViewModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }
            var result = await _userServices.RegisterUser(model);
            logger.LogInformation($"RegisterViewModel: {JsonConvert.SerializeObject(result)}");
            if (result!=null)
            {
                return RedirectToAction("Index", "AdminPanel");
            }
            
            ModelState.AddModelError("", "Invalid Registration");
            return View(model);
        }

        public async Task<IActionResult> DueOrders()
        {
            return View();
        }
        public async Task<IActionResult> PayDue(string id)
        {
            var model = await _adminPanelService.GetOrderViewModel(id);
            return View(model);
        }

        public async Task<IActionResult> PayDueApi()
        {
            var due = Request.Form["due"].ToString();
            var orderid = Request.Form["orderid"].ToString();
            Debug.Print(due + " 0 " + orderid);

            await _adminPanelService.PayDue(orderid, Double.Parse(due));
            return Json(new { status = "Success", orderId = orderid });
        }

        public IActionResult Dues()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                logger.LogInformation($"sortColumn: {sortColumn}");
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                logger.LogInformation($"sortColumnDirection: {sortColumnDirection}");
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data
                var productData = _adminPanelService.GetAllDueOrders();



                /// Todo: Sorting
                /// 

                // Sorting
                // if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                // {
                //     productData = productData.OrderBy(sortColumn + " " + sortColumnDirection);
                // }
                // Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    productData = productData.Where(m => m.CustomerName.ToUpper().Contains(searchValue.ToUpper()));
                }

                //total number of rows count   
                recordsTotal = productData.Count();
                //Paging   
                var data = productData.Skip(skip).Take(pageSize).ToList();
                logger.LogInformation($"Data: {JsonConvert.SerializeObject(data)}");
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Products Endpoint Failed: {ex.Message}");
                return BadRequest("Exception occured");
            }
        }

        public async Task<IActionResult> ProfitCalculation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProfitCalculation(FromToDate dateRange)
        {
            if (ModelState.IsValid == false)
            {
                return View(dateRange);
            }
            Debug.Print(dateRange.ToDateTime.ToString() + " at controller");
            return RedirectToAction("BusinessStatus",new {valf = dateRange.FromDateTime.ToString(),valt = dateRange.ToDateTime.ToString()});
        }

        public async Task<IActionResult> BusinessStatus(string valf,string valt)
        {
            FromToDate val = new FromToDate();
            val.FromDateTime = DateTime.Parse(valf);
            val.ToDateTime = DateTime.Parse(valt);
            val.ToDateTime.AddHours(23);
            var res = await _adminPanelService.BusinessStatus(val);
            return View(res);
        }
        public async Task<IActionResult> ProductSaleSearch()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductSaleSearch(FromToDate dateRange)
        {
            if (ModelState.IsValid == false)
            {
                return View(dateRange);
            }
            Debug.Print(dateRange.ToDateTime.ToString() + " at controller");
            return RedirectToAction("ProductSaleStatus", new { valf = dateRange.FromDateTime.ToString(), valt = dateRange.ToDateTime.ToString() });

        }
        public async Task<IActionResult> ProductSaleStatus(string valf, string valt)
        {
            TempData["fromDate"] = valf;
            TempData["toDate"] = valt;
            return View();
        }
        public async Task<IActionResult> ProductSale()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                logger.LogInformation($"sortColumn: {sortColumn}");
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                logger.LogInformation($"sortColumnDirection: {sortColumnDirection}");
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                var dateRangr = new FromToDate();
                if (TempData["fromDate"] != null)
                {
                    var frm = TempData["fromDate"] as string;
                    dateRangr.FromDateTime = DateTime.Parse(frm);
                }
                if (TempData["toDate"] != null)
                {
                    var to = TempData["toDate"] as string;
                    dateRangr.ToDateTime = DateTime.Parse(to);
                }

                var ress = await _adminPanelService.ProductSaleStatus(dateRangr);
                var ls = ress.AsQueryable();

                /// Todo: Sorting
                // Sorting
                // if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                // {
                //     productData = productData.OrderBy(sortColumn + " " + sortColumnDirection);
                // }
                // Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    ls = ls.Where(m => m.ProductName.ToUpper().Contains(searchValue.ToUpper()));
                }

                //total number of rows count   
                recordsTotal = ls.Count();

                //Paging   
                var data = ls.Skip(skip).Take(pageSize).ToList();
                logger.LogInformation($"Data: {JsonConvert.SerializeObject(data)}");
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Products Endpoint Failed: {ex.Message}");
                return BadRequest("Exception occured");
            }
        }

        

        public async Task<IActionResult> DailyCostingSearch()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DailyCostingSearch(FromToDate dateRange)
        {
            if (ModelState.IsValid == false)
            {
                return View(dateRange);
            }
            Debug.Print(dateRange.ToDateTime.ToString() + " at controller");
            return RedirectToAction("ProductSaleStatus", new { valf = dateRange.FromDateTime.ToString(), valt = dateRange.ToDateTime.ToString() });

        }

        public async Task<IActionResult> DailyCosting(string valf, string valt)
        {
            FromToDate val = new FromToDate();
            val.FromDateTime = DateTime.Parse(valf);
            val.ToDateTime = DateTime.Parse(valt);
            val.ToDateTime.AddHours(23);

            var lst = await _adminPanelService.CostStatus(val);
            var res = new DailyCostStatus();
            res.DailyCosts = lst;
            return View(res);
        }


    }
}