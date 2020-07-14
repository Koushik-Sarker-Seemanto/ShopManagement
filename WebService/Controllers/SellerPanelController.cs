using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebService.Controllers
{
    [Authorize(Policy = "Seller")]
    public class SellerPanelController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}