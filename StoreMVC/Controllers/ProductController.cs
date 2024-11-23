using Microsoft.AspNetCore.Mvc;
using StoreMVC.Data;
using StoreMVC.Models.Entities;

namespace StoreMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDBContext context;
		private readonly IWebHostEnvironment environment;

		public ProductController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
			this.environment = environment;
		}
        public IActionResult ShowProducts()
        {
            var products = context.Products.OrderByDescending(p=>p.Id).ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Create(ProductDetails productDetails)
		{
            if (productDetails.ImageFile == null) 
            {
                ModelState.AddModelError("ImageFile", "The Image File is requires");
            }

            if (!ModelState.IsValid) 
            {
                return View(productDetails);
            }

			//save image file into Products folder
			string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDetails.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDetails.ImageFile.CopyTo(stream);
            }

            //save the new product in the database
            Product product = new Product()
            {
                Name = productDetails.Name,
                Brand = productDetails.Brand,
                Category = productDetails.Category,
                Price = productDetails.Price,
                Description = productDetails.Description,
                ImageFileName = newFileName,
                CreateDate = DateTime.Now,
            };

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("ShowProducts", "Product");
		}


        public IActionResult Edit(int id) 
        {
            var product = context.Products.Find(id);

            if (product == null) 
            {
                return RedirectToAction("ShowProducts", "Product");
            }

            var productDetails = new ProductDetails()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreateAt"] = product.CreateDate.ToString("MM/dd/yyyy");


            return View(productDetails);

        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDetails productDetails)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                return RedirectToAction("ShowProducts", "Product");
            }

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreateAt"] = product.CreateDate.ToString("MM/dd/yyyy");

                return View(productDetails);
            }

            //update image 
            string newFileName = product.ImageFileName;
            if (productDetails.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDetails.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/Products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDetails.ImageFile.CopyTo(stream);
                }

                //delete old image
                string oldImgeFullPath = environment.WebRootPath + "/Products/" + product.ImageFileName;
                System.IO.File.Delete(oldImgeFullPath);
            }

            //update the product in DB
            product.Name = productDetails.Name;
            product.Brand = productDetails.Brand;
            product.Category = productDetails.Category;
            product.Price = productDetails.Price;
            product.Description = productDetails.Description;
            product.ImageFileName = newFileName;

            context.SaveChanges();

			return RedirectToAction("ShowProducts", "Product");
		}

        public IActionResult Delete(int id) 
        {
            var product = context.Products.Find(id);
            if (product == null) 
            {
				return RedirectToAction("ShowProducts", "Product");
			}

			string imageFullPath = environment.WebRootPath + "/Products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);
            context.SaveChanges();

			return RedirectToAction("ShowProducts", "Product");

		}
	}
}
