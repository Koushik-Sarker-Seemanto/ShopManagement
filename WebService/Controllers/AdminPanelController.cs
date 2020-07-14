using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
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
    }
}