using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
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
                ProductsList = productsList.ToList()
            };
            return View(ProductUserVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM productUserVm )
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                                                                 + "templates" + Path.DirectorySeparatorChar.ToString()
                                                                 + "inquiry.html";

            var subject = "New Inquiry";
            string HtmlBody = "";

            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            //Name: { 0}
            //Email: { 1}
            //Phone: { 2}
            //Products: {3}

            StringBuilder productListStringBuilder = new StringBuilder();

            foreach (var product in productUserVm.ProductsList)
            {
                productListStringBuilder.Append($" - Name: {product.Name} <span style='font-size:14px;'> (ID: {product.Id})</span><br />");
            }

            string messageBody = string.Format(HtmlBody,
                ProductUserVm.ApplicationUser.FullName,
                ProductUserVm.ApplicationUser.Email,
                ProductUserVm.ApplicationUser.PhoneNumber,
                productListStringBuilder.ToString());

            await _emailSender.SendEmailAsync(WebConstants.EmailAdmin, subject, messageBody);

            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(ProductUserVM productUserVm)
        {
            HttpContext.Session.Clear();
            return View();
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
