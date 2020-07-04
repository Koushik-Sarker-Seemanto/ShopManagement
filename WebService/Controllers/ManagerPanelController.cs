using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Authorize(Policy = "Manager")]
    public class ManagerPanelController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}