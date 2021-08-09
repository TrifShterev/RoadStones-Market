using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadStones_Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RoadStones_Data.Data;
using RoadStones_Data.Data.Repository.IRepository;
using RoadStones_Models.ViewModels;
using RoadStones_Utility;

namespace RoadStones_Market.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;


        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            HomeVM homeViewModel = new HomeVM()
            {
                Products = _productRepository.GetAll(includeProperties:WebConstants.CategoryName),
                Categories = _categoryRepository.GetAll()
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
                Product = _productRepository.FirstOrDefault(d => d.Id == id, includeProperties: WebConstants.CategoryName),
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

        public IActionResult DetailsPost(int id,DetailsVM detailsVm)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null 
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstants.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart
            {
                ProductId = id,
                SqMeters = detailsVm.Product.TempSqMeters
            });
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            TempData[WebConstants.Success] = "Item add to cart successfully!";

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
