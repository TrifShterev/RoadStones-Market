using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoadStones_Market.Data;
using RoadStones_Market.Models;
using RoadStones_Market.Models.ViewModels;
using RoadStones_Utility;

namespace RoadStones_Market.Controllers
{
    [Authorize(WebConstants.AdminRole)]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> dbListOfProducts = _db.Products.Include(p=> p.Category);

            //foreach (var product in dbListOfProducts)
            //{
            //    product.Category = _db.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            //}

            return View(dbListOfProducts);
        }

        //Get
        public IActionResult CreateUpdate(int? id)
        {
            //ViewBag Solution - NOT RECOMENDED but works
            //IEnumerable<SelectListItem> CategoryDropDown = _db.Categories
            //    .Select(x => new SelectListItem
            //    {
            //        Text = x.Name,
            //        Value = x.Id.ToString()
            //    });

            //ViewBag.CategoryDropDown = CategoryDropDown;

            //var product = new Product();


            //ViewModel solution -RECOMMENDED
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategorySelectList = _db.Categories
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
            };

            if (id==null)
            {
                //this is for Create
                return View(productVM);
            }
            else
            {
                productVM.Product = _db.Products.Find(id.GetValueOrDefault());

                if (productVM.Product== null)
                {
                    return NotFound();
                }

                return View(productVM);
            }
            
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM productVm)
        {
            //Server Validation- Check
            if (!ModelState.IsValid)
            {
                productVm.CategorySelectList = _db.Categories
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
                return View(productVm);
            }

            var files = HttpContext.Request.Form.Files;

            string webRoothPath = _webHostEnvironment.WebRootPath;

            if (productVm.Product.Id == 0)
            {
                //Creating (Adds new file-product and picture to the Db and saves it at \wwwroot\images\products)
                string uploadPath = webRoothPath + WebConstants.ImagePath;

                string fileName = Guid.NewGuid().ToString();

                string extentionOfTheFile = Path.GetExtension(files[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(uploadPath,fileName+extentionOfTheFile),FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }

                productVm.Product.Image = fileName + extentionOfTheFile;

                _db.Products.Add(productVm.Product);
                
            }
            else
            {
                //Updating
                var productFromDb = _db.Products.AsNoTracking().FirstOrDefault(p => p.Id == productVm.Product.Id);

                if (productFromDb == null)
                {
                    return NotFound();
                }

                if (files.Count>0)
                {
                    string uploadPath = webRoothPath + WebConstants.ImagePath;

                    string fileName = Guid.NewGuid().ToString();

                    string extentionOfTheFile = Path.GetExtension(files[0].FileName);

                    var oldFile = Path.Combine(uploadPath, productFromDb.Image);

                    if (System.IO.File.Exists(oldFile))
                    {
                       System.IO.File.Delete(oldFile);
                    }
                    

                    using (var fileStream = new FileStream(Path.Combine(uploadPath, fileName + extentionOfTheFile), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    productVm.Product.Image = fileName + extentionOfTheFile;

                }
                else
                {
                    productVm.Product.Image =productFromDb.Image;
                }

                _db.Products.Update(productVm.Product);
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        

        //GET
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var product = _db.Products.Include(x=>x.Category).FirstOrDefault(x=>x.Id==id);
            
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //Post
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var model = _db.Products.Find(id);

            //Server Validation- Check
            if (model== null)
            {
                return NotFound();
            }

            string uploadPath = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;
            
            var oldFile = Path.Combine(uploadPath, model.Image);

            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }

            _db.Products.Remove(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
