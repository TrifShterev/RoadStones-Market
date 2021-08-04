using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using RoadStones_Data.Data.Repository.IRepository;
using RoadStones_Models;
using RoadStones_Models.ViewModels;
using WebConstants = RoadStones_Utility.WebConstants;
using RoadStones_Utility;

namespace RoadStones_Market.Controllers
{
    [Authorize(WebConstants.AdminRole)]
    public class InquiryController : Controller
    {
        [BindProperty]
        public InquiryVM InquiryVm { get; set; }

        private readonly IInquiryHeaderRepository _inquiryHeader;
        private readonly IInquiryDetailsRepository _inquiryDetails;

        public InquiryController(IInquiryDetailsRepository inquiryDetails, IInquiryHeaderRepository inquiryHeader)
        {
            _inquiryDetails = inquiryDetails;
            _inquiryHeader = inquiryHeader;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            InquiryVm = new InquiryVM()
            {
                InquiryHeader = _inquiryHeader.FirstOrDefault(u => u.Id == id),
                InquiryDetails = _inquiryDetails.GetAll(u => u.InquiryHeaderId == id, includeProperties: "Product")
            };

            return View(InquiryVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();

            InquiryVm.InquiryDetails = _inquiryDetails.GetAll(u => u.InquiryHeaderId == InquiryVm.InquiryHeader.Id);

            foreach (var detail in InquiryVm.InquiryDetails )
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };

                shoppingCartList.Add(shoppingCart);
            }

            HttpContext.Session.Clear();
            HttpContext.Session.Set(WebConstants.SessionCart,shoppingCartList);
            HttpContext.Session.Set(WebConstants.SessionInquiryId,InquiryVm.InquiryHeader.Id);

            return RedirectToAction("Index","Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inquiryHeader.FirstOrDefault(u => u.Id == InquiryVm.InquiryHeader.Id);

            IEnumerable<InquiryDetails> inquiryDetailsEnumerable =
                _inquiryDetails.GetAll(u => u.InquiryHeaderId == InquiryVm.InquiryHeader.Id);

            _inquiryDetails.RemoveRange(inquiryDetailsEnumerable);
            _inquiryHeader.Remove(inquiryHeader);
            _inquiryHeader.Save();

            return RedirectToAction(nameof(Index));
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new {data = _inquiryHeader.GetAll()});
        }


        #endregion

    }
}
