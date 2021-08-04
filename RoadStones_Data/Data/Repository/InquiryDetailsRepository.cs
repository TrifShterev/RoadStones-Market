using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoadStones_Data.Data.Repository.IRepository;
using RoadStones_Models;
using System.Collections.Generic;
using System.Linq;
using RoadStones_Utility;

namespace RoadStones_Data.Data.Repository
{
    public class InquiryDetailsRepository : Repository<InquiryDetails>, IInquiryDetailsRepository
    {
        private readonly ApplicationDbContext _db;
        
        public InquiryDetailsRepository( ApplicationDbContext db) : base( db)
        {
            _db = db;
        }

        public void Update(InquiryDetails obj)
        {
            //if we want to update all the props of the product 
            _db.InquiryDetails.Update(obj);
        }

        
    }
}