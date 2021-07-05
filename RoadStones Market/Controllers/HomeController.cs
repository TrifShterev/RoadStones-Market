using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RoadStones_Market.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RoadStones_Market.Data;
using RoadStones_Market.Models.ViewModels;

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
            var detailsVm = new DetailsVM()
            {
                Product = _db.Products
                    .Include(u => u.Category)
                    .FirstOrDefault(d => d.Id == id),
                ExistInCart = false

            };

            return View(detailsVm);
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
