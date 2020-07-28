using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
using Models.Entities;
using Models.ManagerPanelModels;
using Newtonsoft.Json;
using Services.Contracts;
using System.Linq.Dynamic.Core;
using Models.CommonEnums;

namespace WebService.Controllers
{
    [Authorize(Policy = "Manager")]
    public class ManagerPanelController : Controller
    {
        private IUserServices _userServices;
        private ILogger<ManagerPanelController> logger;

        private readonly IManagerPanelService _managerPanelService;

        public ManagerPanelController(IUserServices userServices, ILogger<ManagerPanelController> logger, IManagerPanelService managerPanelService)
        {
            _userServices = userServices;
            this.logger = logger;
            _managerPanelService = managerPanelService;
        }
        // GET
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult AddSeller()
        {
            return View();
        }
        


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSeller([Bind] RegisterViewModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }

            model.Role = Roles.Seller;
            var result = await _userServices.RegisterUser(model);
            logger.LogInformation($"RegisterViewModel: {JsonConvert.SerializeObject(result)}");
            if (result!=null)
            {
                return RedirectToAction("Index", "ManagerPanel");
            }
            
            ModelState.AddModelError("", "Invalid Registration");
            return View(model);
        }

        public IActionResult AddProduct()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind] ProductViewModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }
            var result = await _managerPanelService.AddProduct(model);
            logger.LogInformation($"Product: {JsonConvert.SerializeObject(model)}");
            if (result != null)
            {
                return RedirectToAction("Index", "ManagerPanel");
            }
            ModelState.AddModelError("", "Invalid Product");
            return View(model);
        }

        public IActionResult Products()
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
                var productData = _managerPanelService.GetAllProducts();

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    productData = productData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    productData = productData.Where(m => m.Name.ToUpper().Contains(searchValue.ToUpper()));
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
        public IActionResult StockCheck()
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
                var productData = _managerPanelService.GetAllStockEnd();

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                {
                    productData = productData.OrderBy(sortColumn + " " + sortColumnDirection);
                }
                //Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    productData = productData.Where(m => m.Name.ToUpper().Contains(searchValue.ToUpper()));
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

        [HttpGet]
        public async Task<IActionResult> ProductDetails(string productId)
        {
            var product = await _managerPanelService.FindProductById(productId);
            
            return View(product);
        }
        public async Task<IActionResult> StockRemaining()
        {
            return View();
        }

        public async Task<IActionResult> GenerateBarcode(string productId, int stock, double buyingPrice)
        {
            logger.LogInformation($" ---------------------------- {stock} ------------ {productId}");
            //Debug.Print(" ---------------------------- " + stock + "------------" + productId);
            var resProduct = await _managerPanelService.UpdateCurrentStock(productId, stock, buyingPrice);
            if (resProduct == null)
            {
                //Error Should be raised.
                return RedirectToAction("ProductDetails", "ManagerPanel", new {productId = productId});
            }
            return RedirectToAction("ProductDetails", "ManagerPanel", new {productId = productId});
        }
    }
}