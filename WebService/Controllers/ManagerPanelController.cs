using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize(Policy = "Manager")]
    public class ManagerPanelController : Controller
    {
        private IUserServices _userServices;
        private ILogger<ManagerPanelController> logger;
        //private IAdminPanelService _adminPanelService;

        public ManagerPanelController(IUserServices userServices, ILogger<ManagerPanelController> logger)
        {
            _userServices = userServices;
            this.logger = logger;
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
    }
}