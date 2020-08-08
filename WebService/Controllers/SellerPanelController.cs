using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.ViewModels.SellerPanel;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize(Policy = "Seller")]
    public class SellerPanelController : Controller
    {
        private ISellerPanelService _sellerPanelService;
        private ILogger<SellerPanelController> logger;
        public SellerPanelController(ISellerPanelService sellerPanelService, ILogger<SellerPanelController> logger)
        {
            _sellerPanelService = sellerPanelService;
            this.logger = logger;
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
                    if(res.Sold)
                        return Json(new { res = res, status = "Sold" });

                    return Json(new {res = res, status = "Found"});
                }
            }

            return Json(new {status = "Not Found" });
        }

        [HttpPost]
        public async Task<IActionResult> SellProduct()
        {
            var order = Request.Form["order"].ToArray();
            var name = Request.Form["name"].ToArray();
            var phone = Request.Form["phone"].ToArray();
            logger.LogInformation($"Orderssssssssssss: {JsonConvert.SerializeObject(order)}");
            logger.LogInformation($"nameeeeeeeeeeee: {JsonConvert.SerializeObject(name)}");
            logger.LogInformation($"Phoneeeeeeeeeeee: {JsonConvert.SerializeObject(phone)}");
            return Json(ne

        }
    }
}