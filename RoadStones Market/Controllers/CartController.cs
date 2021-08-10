﻿using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using RoadStones_Data.Data;
using RoadStones_Data.Data.Repository.IRepository;
using RoadStones_Models;
using RoadStones_Models.ViewModels;
using RoadStones_Utility;
using RoadStones_Utility.BrainTree;

namespace RoadStones_Market.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        [BindProperty] 
        public ProductUserVM ProductUserVm { get; set; }

        
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationUserRepository _applicationUser;
        private readonly IProductRepository _productRepository;
        private readonly IInquiryHeaderRepository _inquiryHeader;
        private readonly IInquiryDetailsRepository _inquiryDetails;
        private readonly IOrderHeaderRepository _orderHeaderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IBrainTreeGate _brain;
        

        public CartController( IWebHostEnvironment webHostEnvironment, IEmailSender emailSender, IApplicationUserRepository applicationUser, IProductRepository productRepository, IInquiryHeaderRepository inquiryHeader, IInquiryDetailsRepository inquiryDetails, IOrderDetailRepository orderDetailRepository, IOrderHeaderRepository orderHeaderRepository, IBrainTreeGate brain)
        {
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender;
            _applicationUser = applicationUser;
            _inquiryHeader = inquiryHeader;
            _inquiryDetails = inquiryDetails;
            _orderDetailRepository = orderDetailRepository;
            _orderHeaderRepository = orderHeaderRepository;
            _brain = brain;
            _productRepository = productRepository;
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

            IEnumerable<Product> productsListTemp = _productRepository.GetAll(p => productsIdsInCart.Contains(p.Id));
            List<Product> productsList= new List<Product>();

            foreach (var shoppingCart in shoppingCartList)
            {
                Product productTemp = productsListTemp.FirstOrDefault(u => u.Id == shoppingCart.ProductId);
                productTemp.TempSqMeters = shoppingCart.SqMeters;
                productsList.Add(productTemp);
            }
            
            return View(productsList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            
            foreach (var product in prodList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    ProductId = product.Id,
                    SqMeters = product.TempSqMeters
                });
            }
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Summary));
        }

        public IActionResult Summary()
        {
            ApplicationUser appUser;

            if (User.IsInRole(WebConstants.AdminRole))
            {
                if (HttpContext.Session.Get<int>(WebConstants.SessionInquiryId) != 0)
                {
                    //cart has been loaded using an inquiry
                    InquiryHeader inquiryHeader = _inquiryHeader.FirstOrDefault(u =>
                        u.Id == HttpContext.Session.Get<int>(WebConstants.SessionInquiryId));

                    appUser = new ApplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    appUser = new ApplicationUser();
                }

                var gateway = _brain.GetGateway();
                var clientToken = gateway.ClientToken.Generate();

                ViewBag.ClientToken = clientToken;

            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;

                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                appUser = _applicationUser.FirstOrDefault(u => u.Id == claim.Value);
            }

           

            // var userId = User.FindFirstValue(ClaimTypes.Name); --> Another way to retrive UserId

            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).Any())
            {
                //Session exists
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstants.SessionCart).ToList();
            }

            List<int> productsIdsInCart = shoppingCartList.Select(x => x.ProductId).ToList();

            IEnumerable<Product> productsList = _productRepository.GetAll(p => productsIdsInCart.Contains(p.Id));

            ProductUserVm = new ProductUserVM()
            {
                ApplicationUser = appUser,
               
            };

            foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = _productRepository.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempSqMeters = cartObj.SqMeters;
                ProductUserVm.ProductsList.Add(prodTemp);
            }

            return View(ProductUserVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection,ProductUserVM productUserVm )
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            if (User.IsInRole(WebConstants.AdminRole))
            {
                //we need to create an order

               

                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value,
                    FinalOrderTotal = productUserVm.ProductsList.Sum(p=>p.TempSqMeters*p.Price),
                    City = productUserVm.ApplicationUser.City,
                    StreetAddress = productUserVm.ApplicationUser.StreetAddress,
                    Country = productUserVm.ApplicationUser.Country,
                    PostalCode = productUserVm.ApplicationUser.PostalCode,
                    FullName = productUserVm.ApplicationUser.FullName,
                    Email = productUserVm.ApplicationUser.Email,
                    PhoneNumber = productUserVm.ApplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WebConstants.StatusPending

                };
                _orderHeaderRepository.Add(orderHeader);
                _orderHeaderRepository.Save();

                foreach (var product in productUserVm.ProductsList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerSqMeter = product.Price,
                        SquareMeter = product.TempSqMeters,
                        ProductId = product.Id
                    };

                    _orderDetailRepository.Add(orderDetail);
                }

                _orderDetailRepository.Save();

                //BrainTree payment transaction request
                string nonceFromTheClient = collection["payment_method_nonce"];

                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal),
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),
                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true
                    }
                };
                var gateway = _brain.GetGateway();

                Result<Transaction> result = gateway.Transaction.Sale(request);

                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WebConstants.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WebConstants.StatusCancelled;
                }

                _orderHeaderRepository.Save();

                return RedirectToAction(nameof(InquiryConfirmation), new {id=orderHeader.Id});
            }
            else
            {
                //we need to create an inquiry


                var pathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                                                                     + "templates" + Path.DirectorySeparatorChar.ToString()
                                                                     + "inquiry.html";

                var subject = "New Inquiry";
                string HtmlBody = "";

                using (StreamReader sr = System.IO.File.OpenText(pathToTemplate))
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


                //this must be pushed to DB together with InquiryDetails
                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    ApplicationUserId = claim.Value,
                    FullName = ProductUserVm.ApplicationUser.FullName,
                    Email = ProductUserVm.ApplicationUser.Email,
                    PhoneNumber = ProductUserVm.ApplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now

                };

                _inquiryHeader.Add(inquiryHeader);
                _inquiryHeader.Save();

                foreach (var product in productUserVm.ProductsList)
                {
                    InquiryDetails inquiryDetail = new InquiryDetails()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id
                    };

                    _inquiryDetails.Add(inquiryDetail);
                }

                _inquiryDetails.Save();

                TempData[WebConstants.Success] = "Inquiry added Successfully!";
            }




            return RedirectToAction(nameof(InquiryConfirmation));
        }

        public IActionResult InquiryConfirmation(int id=0)
        {
            OrderHeader orderHeader = _orderHeaderRepository.FirstOrDefault(u => u.Id == id);
            HttpContext.Session.Clear();
            return View(orderHeader);
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

           TempData[WebConstants.Success] = "Item Removed Successfully!";


            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            foreach (var product in prodList)
            {
                shoppingCartList.Add(new ShoppingCart
                {
                    ProductId = product.Id,
                    SqMeters = product.TempSqMeters
                });
            }
            HttpContext.Session.Set(WebConstants.SessionCart, shoppingCartList);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Clear()
        {
            HttpContext.Session.Clear();


            return RedirectToAction("Index", "Home");
        }
    }
}
