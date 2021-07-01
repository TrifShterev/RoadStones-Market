using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoadStones_Market.Data;
using RoadStones_Market.Models;
using RoadStones_Market.Models.ViewModels;

namespace RoadStones_Market.Controllers
{
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
            IEnumerable<Product> objList = _db.Products;

            foreach (var product in objList)
            {
                product.Category = _db.Categories.FirstOrDefault(c => c.Id == product.CategoryId);
            }

            return View(objList);
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
                productVM.Product = _db.Products.Find(id);

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
            var model = _db.Products.Find(id);

            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var model = _db.Products.Find(id);

            //Server Validation- Check
            if (model== null)
            {
                return NotFound();
            }

            _db.Products.Remove(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
