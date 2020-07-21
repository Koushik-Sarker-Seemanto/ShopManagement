using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.SellerPanel;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize(Policy = "Seller")]
    public class SellerPanelController : Controller
    {
        private ISellerPanelService _sellerPanelService;
        public SellerPanelController(ISellerPanelService sellerPanelService)
        {
            _sellerPanelService = sellerPanelService;
        }
        // GET
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetProduct(string id)
        {
            Debug.Print(id + "---------------------->");
            if (id != null)
            {
                var res = await _sellerPanelService.GetProductFromBar(id);
                if (res != null)
                {
                    Debug.Print("a " +res.ProductTitle );
                    
                    return Json(new {res = res, status = "Found"});
                }
            }

            return Json(new {status = "Not Found" });
        }
    }
}