using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoadStones_Data.Data.Repository.IRepository;

namespace RoadStones_Market.Controllers
{

    public class InquiryController : Controller
    {
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
    }
}
