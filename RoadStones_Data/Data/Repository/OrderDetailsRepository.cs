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
    public class OrderDetailsRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        
        public OrderDetailsRepository( ApplicationDbContext db) : base( db)
        {
            _db = db;
        }

        public void Update(OrderDetail obj)
        {
            //if we want to update all the props of the product 
            _db.OrderDetails.Update(obj);
        }

        
    }
}