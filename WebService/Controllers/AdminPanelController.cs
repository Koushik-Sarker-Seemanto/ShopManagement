using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.AdminAuthModels;
using Newtonsoft.Json;

namespace WebService.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminPanelController : Controller
    {
        private ILogger<AdminPanelController> logger;
        public AdminPanelController(ILogger<AdminPanelController> logger)
        {
            this.logger = logger;
        }
        // GET
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddEmployee([Bind] RegisterViewModel model)
        {
            if(ModelState.IsValid == false)
            {
                return View(model);
            }
            //TODO: AdminPanelService need to be call.
            logger.LogInformation($"RegisterViewModel: {JsonConvert.SerializeObject(model)}");
            return RedirectToAction("Index", "AdminPanel");
        }
    }
}