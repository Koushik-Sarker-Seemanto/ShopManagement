using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminPanelController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}