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
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;
        
        public InquiryHeaderRepository( ApplicationDbContext db) : base( db)
        {
            _db = db;
        }

        public void Update(InquiryHeader obj)
        {
            //if we want to update all the props of the product 
            _db.InquiryHeader.Update(obj);
        }

        
    }
}