using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

using RoadStones_Data.Data.Repository.IRepository;
using RoadStones_Models;
using RoadStones_Utility;

namespace RoadStones_Market.Controllers
{
    [Authorize(WebConstants.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryService;

        public CategoryController(ICategoryRepository categoryService)
        {
            this._categoryService = categoryService;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> objList = _categoryService.GetAll();

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
                TempData[WebConstants.Error] = "Error while creating category!";
                return View(model);
            }

            _categoryService.Add(model);
            _categoryService.Save();

            TempData[WebConstants.Success] = "Category Created Successfully!";

            return RedirectToAction("Index");
        }

        //GET
        public IActionResult Edit(int? id)
        {
            if (id==0 || id == null)
            {
                return NotFound();
            }
            var model = _categoryService.Find(id.GetValueOrDefault());

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
                TempData[WebConstants.Error] = "Error while editing category!";
                return View(model);
            }

            _categoryService.Update(model);
            _categoryService.Save();

            TempData[WebConstants.Success] = "Category Edited Successfully!";

            return RedirectToAction("Index");
        }
        //GET
        public IActionResult Delete(int? id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var model = _categoryService.Find(id.GetValueOrDefault());

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
            var model = _categoryService.Find(id.GetValueOrDefault());

            //Server Validation- Check
            if (model== null)
            {
                return NotFound();
            }

            _categoryService.Remove(model);
            _categoryService.Save();

            TempData[WebConstants.Success] = "Category Deleted Successfully!";
            return RedirectToAction("Index");
        }
    }
}
