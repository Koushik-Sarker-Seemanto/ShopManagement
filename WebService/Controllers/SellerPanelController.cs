using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
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
                    
                    return Json(new {res = res, status = "Found"});
                }
            }

            return Json(new {status = "Not Found" });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProductNonBarcode(string name, string quantity)
        {
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(quantity))
            {
                var res = await _sellerPanelService.GetProductNonBar(name, quantity);
                if (res != null)
                {
                    return Json(new {res = res, status = "Found"});
                }
            }

            return Json(new {status = "Not Found" });
        }
        
        [HttpGet]
        public async Task<IActionResult> GetProductByName(string query)
        {
            logger.LogInformation($"Queryyyyyyyy: {query}");
            if (query != null)
            {
                var res = await _sellerPanelService.GetProductByName(query);
                if (res != null)
                {
                    logger.LogInformation($"Suggestion Listtttt: {JsonConvert.SerializeObject(res)}");
                    
                    return Json(new {res = JsonConvert.SerializeObject(res), status = "Found"});
                }
            }

            return Json(new {status = "Not Found" });
        }

        //This endpoint is temporary for easy testing purpose.
        [HttpGet]
        public async Task<IActionResult> TempProductList()
        {
            var response = _sellerPanelService.GetAllProducts();
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> SellProduct()
        {
            try
            {
                var orderData = Request.Form["order"];
                var order = JsonConvert.DeserializeObject<List<string>>(orderData);
                var orderDataNonBar = Request.Form["orderNonBar"];
                var orderNonBar = JsonConvert.DeserializeObject<List<NonBar>>(orderDataNonBar);
                var name = Request.Form["name"].ToString();
                var phone = Request.Form["phone"].ToString();
                var discount = Request.Form["discount"].ToString();
                var totalPrice = Request.Form["totalAmount"].ToString();
                var due = Request.Form["due"].ToString();
                logger.LogInformation($"Orderssssssssssss: {JsonConvert.SerializeObject(order)}");
                logger.LogInformation($"Orderssssssssssss: {JsonConvert.SerializeObject(orderNonBar)}");
                logger.LogInformation($"nameeeeeeeeeeee: {name}");
                logger.LogInformation($"Phoneeeeeeeeeeee: {phone}");
                logger.LogInformation($"Dsicounttttttttt: {discount}");
                logger.LogInformation($"totalpriceeeeeeeeee: {totalPrice}");
                logger.LogInformation($"dueeeeeeeeeeeeee: {due}");
                OrderViewModel model = new OrderViewModel();
                
                model.Discount = Double.Parse(discount);
                model.TotalPrice = Double.Parse(totalPrice);
                model.DueAmount = Double.Parse(due);
                var products = new List<string>();
                foreach (var item in order)
                {
                    products.Add(item);
                    logger.LogInformation($"ProductttttttttttttttId: {item}");
                }
                var productNonBar = new List<NonBar>();
                foreach (var item in orderNonBar)
                {
                    productNonBar.Add(item);
                    logger.LogInformation($"ProductttttttttttttttId: {item}");
                }

                model.Order = products;
                model.OrderNonBar = productNonBar;
            
                var response = await _sellerPanelService.SellProduct(model);
                if (response != null && !string.IsNullOrEmpty(response.Id))
                {
                    return Json(new {status = "Success", orderId = response.Id});
                }
                return Json(new {status = "Fail"});
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"SellProduct Endpoint Failed: {ex.Message}");
                return Json(new {status = "Fail"});
            }
        }
    }
}