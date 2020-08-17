using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.SellerPanelModels;
using Models.ViewModels.SellerPanel;
using Newtonsoft.Json;
using Services.Contracts;

namespace WebService.Controllers
{
    [Authorize(Policy = "Seller")]
    public class SellerPanelController : Controller
    {
        private ISellerPanelService _sellerPanelService;
        private IAdminPanelService _adminPanelService;
        private ILogger<SellerPanelController> logger;
        public SellerPanelController(IAdminPanelService adminPanelService,ISellerPanelService sellerPanelService, ILogger<SellerPanelController> logger)
        {
            _adminPanelService = adminPanelService;
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
                model.Name = name;
                model.Phone = phone;
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

        public async Task<IActionResult> SearchProduct()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SearchProduct(ProductIdInput product)
        {
            
            return RedirectToAction("ProductDetails","SellerPanel",new { id = product.ProductId});
        }

        public async Task<IActionResult> ProductDetails(string id)
        {
            var inp = new ProductIdInput();
            inp.ProductId = id;
            var model = await _sellerPanelService.GetAllDetail(inp);
            logger.LogInformation(model.Product.Id+"++++++++++++++++++++++++++");
            return View(model);
        }

        public async Task<IActionResult> DueOrders()
        {
            return View();
        }
        public async Task<IActionResult> PayDue(string id)
        {
            var model = await _adminPanelService.GetOrderViewModel(id);
            return View(model);
        }

        public async Task<IActionResult> PayDueApi()
        {
            var due = Request.Form["due"].ToString();
            var orderid = Request.Form["orderid"].ToString();
            Debug.Print(due + " 0 " + orderid);

            await _adminPanelService.PayDue(orderid, Double.Parse(due));
            return Json(new { status = "Success", orderId = orderid });
        }

        public IActionResult Dues()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                // Skiping number of Rows count  
                var start = Request.Form["start"].FirstOrDefault();
                // Paging Length 10,20  
                var length = Request.Form["length"].FirstOrDefault();
                // Sort Column Name  
                var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
                logger.LogInformation($"sortColumn: {sortColumn}");
                // Sort Column Direction ( asc ,desc)  
                var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                logger.LogInformation($"sortColumnDirection: {sortColumnDirection}");
                // Search Value from (Search box)  
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                //Paging Size (10,20,50,100)  
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data
                var productData = _adminPanelService.GetAllDueOrders();



                /// Todo: Sorting
                /// 

                // Sorting
                // if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDirection)))
                // {
                //     productData = productData.OrderBy(sortColumn + " " + sortColumnDirection);
                // }
                // Search
                if (!string.IsNullOrEmpty(searchValue))
                {
                    productData = productData.Where(m => m.CustomerName.ToUpper().Contains(searchValue.ToUpper()));
                }

                //total number of rows count   
                recordsTotal = productData.Count();
                //Paging   
                var data = productData.Skip(skip).Take(pageSize).ToList();
                logger.LogInformation($"Data: {JsonConvert.SerializeObject(data)}");
                //Returning Json Data  
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Products Endpoint Failed: {ex.Message}");
                return BadRequest("Exception occured");
            }
        }
    }
}