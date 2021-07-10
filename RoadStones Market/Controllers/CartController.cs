using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RoadStones_Market.Data;
using RoadStones_Market.Models;
using RoadStones_Market.Models.ViewModels;
using RoadStones_Market.Utility;

namespace RoadStones_Market.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        [BindProperty] 
        public ProductUserVM ProductUserVm { get; set; }

        private readonly ApplicationDbContext _db;

        public CartController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart)!=null
            && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                //Session exists
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).ToList();
            }

            List<int> productsIdsInCart = shoppingCartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> productsList = _db.Products.Where(p => productsIdsInCart.Contains(p.Id));


            return View(productsList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
           
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;

            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            // var userId = User.FindFirstValue(ClaimTypes.Name); --> Another way to retrive UserId

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                //Session exists
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).ToList();
            }

            List<int> productsIdsInCart = shoppingCartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> productsList = _db.Products.Where(p => productsIdsInCart.Contains(p.Id));

            ProductUserVm = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claim.Value),
                ProductsList = productsList
            };


            return View(ProductUserVm);
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                //Session exists
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).ToList();
            }

            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(p => p.ProductId == id));

           HttpContext.Session.Set(WebConstants.SessionCart,shoppingCartList);


            return RedirectToAction(nameof(Index));
        }
    }
}
