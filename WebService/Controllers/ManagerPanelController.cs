using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
using Models.Entities;
using Models.ManagerPanelModels;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize(Policy = "Manager")]
    public class ManagerPanelController : Controller
    {
        private IUserServices _userServices;
        private ILogger<ManagerPanelController> logger;

        private IManagerPanelService _managerPanelService;
        //private IAdminPanelService _adminPanelService;

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
    }
}