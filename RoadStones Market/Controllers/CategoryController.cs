using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using RoadStones_Market.Data;
using RoadStones_Market.Models;
using RoadStones_Utility;

namespace RoadStones_Market.Controllers
{
    [Authorize(WebConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _db.Categories;

            return View(objList);
        }

        //Get
        public IActionResult Create()
        {
            return View();
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category model)
        {
            //Server Validation- Check
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _db.Categories.Add(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //GET
        public IActionResult Edit(int id)
        {
            if (id==0 || id == null)
            {
                return NotFound();
            }
            var model = _db.Categories.Find(id);

            if (model== null)
            {
                return NotFound();
            }
            return View(model);
        }

        //Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category model)
        {
            //Server Validation- Check
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _db.Categories.Update(model);
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
            var model = _db.Categories.Find(id);

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
            var model = _db.Categories.Find(id);

            //Server Validation- Check
            if (model== null)
            {
                return NotFound();
            }

            _db.Categories.Remove(model);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
