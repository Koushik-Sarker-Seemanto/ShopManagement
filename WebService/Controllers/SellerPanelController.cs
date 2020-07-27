using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels.SellerPanel;
using Newtonsoft.Json;
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

        [HttpPost]
        public async Task<IActionResult> SellProduct()
        {
            var orders = Request.Form["order"];
            var order = JsonConvert.DeserializeObject<string[]>(orders);
            var name = Request.Form["name"].ToString();
            var phone = Request.Form["phone"].ToString();
            var paystatus = Request.Form["pay"].ToString();
            var totals = Request.Form["total"].ToString();
            var total = int.Parse(totals);
            var lst = new List<string>();
            Debug.Print(order.Length+"aaa");
            for(int i = 0 ;i<order.Length;i++)
                lst.Add(order[i]);

            var orderModel = new OrderViewModel
            { 
                Order = lst,
                Name = name,
                Phone = phone,
                Amount = total,
            };

            if (paystatus != null)
            {

                if (paystatus == "Due")
                {
                    orderModel.Paid = false;
                }
                else
                {
                    orderModel.Paid = true;
                }
            }
            else
            {
                // Error
            }

            Debug.Print(orderModel.Name);
            var orderRes = await _sellerPanelService.MakeOrder(orderModel);
            if (orderRes == null)
            {
                return Json(new { status = "Fail" });
            }

            return Json(new { status = "Ok" });



        }
    }
}