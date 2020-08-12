using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
using Models.Entities;
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

                //Sorting
                // if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                // {
                //     productData = productData.OrderBy(sortColumn + " " + sortColumnDirection);
                // }
                //Search
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




    }
}