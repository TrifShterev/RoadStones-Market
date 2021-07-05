using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadStones_Market.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RoadStones_Market.Data;
using RoadStones_Market.Models.ViewModels;
using RoadStones_Market.Utility;

namespace RoadStones_Market.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index()
        {
            HomeVM homeViewModel = new HomeVM()
            {
                Products = _db.Products.Include(u => u.Category),
                Categories = _db.Categories
            };

            return View(homeViewModel);
        }

        public IActionResult Details(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }


            var detailsVm = new DetailsVM()
            {
                Product = _db.Products
                    .Include(u => u.Category)
                    .FirstOrDefault(d => d.Id == id),
                ExistInCart = false

            };

            foreach (var item in shoppingCartList)
            {
                if (item.ProductId==id)
                {
                    detailsVm.ExistInCart = true;
                }   
            }

            return View(detailsVm);
        }


        [HttpPost,ActionName("Details")]

        public IActionResult DetailsPost(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart
            {
                ProductId = id
            });
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        

        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }

            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);

            if (itemToRemove!=null)
            {
                shoppingCartList.Remove(itemToRemove);
            }

           
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
