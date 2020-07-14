using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Aspose.BarCode.Generation;
using Microsoft.Extensions.Logging;
using Models.Entities;
using Models.ManagerPanelModels;
using Repositories;
using Services.Contracts;

namespace Services
{
    public class ManagerPanelService: IManagerPanelService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<IManagerPanelService> _logger;
        public ManagerPanelService(IMongoRepository repository, ILogger<IManagerPanelService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<Product> AddProduct(ProductViewModel model)
        {
            try
            {
                if (model != null)
                {
                    var guid = Guid.NewGuid().ToString();
                    var product = new Product
                    {
                        Id = guid,
                        Name = model.Name,
                        BuyingPrice = model.BuyingPrice,
                        SellingPrice = model.SellingPrice,
                        Details = model.Details,
                    };
                    await _repository.SaveAsync<Product>(product);
                    return product;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddProduct Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Product> FindProductById(string modelId)
        {
            try
            {
                if (modelId != null)
                {
                    var pro =  await _repository.GetItemAsync<Product>(d => d.Id == modelId);
                    return pro;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AddProduct Failed: {ex.Message}");
                return null;
            }
        }

        public async Task<Product> UpdateCurrentStock(string productId, int stockAmount)
        {
            try
            {
                var product = await FindProductById(productId);
                product.Stock += stockAmount;
                await _repository.UpdateAsync<Product>(d => d.Id == productId, product);
                GenrateBarCode(productId,stockAmount,product.Name);
                return product;
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Product Update Failed : {e.Message}");
                return null;
            }
        }


        private void GenrateBarCode(string productId,int stock,string title)
        {


            Dictionary<string, BaseEncodeType> collection = new Dictionary<string, BaseEncodeType>();
            collection.Add(productId, EncodeTypes.Code11);
            List<Bitmap> images = new List<Bitmap>();

            foreach (KeyValuePair<string, BaseEncodeType> pair in collection)
            {
                for (int i = 0; i < stock; i++)
                {
                    BarcodeGenerator builder = new BarcodeGenerator(pair.Value, pair.Key);
                    images.Add(builder.GenerateBarCodeImage());
                }
            }

            int maxWidth = int.MinValue;
            int sumHeight = 0;
            foreach (Bitmap bmp in images)
            {
                sumHeight += bmp.Height;
                if (maxWidth < bmp.Width)
                    maxWidth = bmp.Width;
            }

            const int offset = 10;
            Bitmap resultBitmap = new Bitmap(maxWidth + offset * 2, sumHeight + offset * images.Count);
            using (Graphics g = Graphics.FromImage(resultBitmap))
            {
                g.Clear(Color.White);

                int yPosition = offset;
                Font drawFont = new Font("Arial", 16);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                // Todo: Fix Product Title Part
                g.DrawString("Product Name :  " + title, drawFont, drawBrush, offset, yPosition);
                yPosition += offset+20;
                for (int i = 0; i < images.Count; ++i)
                {
                    Bitmap currentBitmap = images[i];
                    g.DrawImage(currentBitmap, offset, yPosition);
                    yPosition += currentBitmap.Height + offset;
                }
            }

            var name =  title+" "+ DateTime.Now.ToString("MMMM dd HH-mm tt") + ".png";
            // Debug.Print(DateTime.Now.ToString("MMMM dd HH-mm tt"));
            
            resultBitmap.Save($"wwwroot\\images\\" + name, ImageFormat.Png);
            
        }

        public IQueryable<Product> GetAllProducts()
        {
            try
            {
                var results = _repository.GetItems<Product>();
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetAllProducts Failed: {ex.Message}");
                return null;
            }
        }
    }
}