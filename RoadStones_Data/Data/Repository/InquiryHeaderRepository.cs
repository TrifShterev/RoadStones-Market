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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        
        public ProductRepository( ApplicationDbContext db) : base( db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetAllDropdownItems(string obj)
        {
            if (obj == WebConstants.CategoryName)
            {
                return _db.Categories
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    });
            }

            return null;
        }

        public void Update(Product obj)
        {
            //if we want to update all the props of the product 
            _db.Products.Update(obj);
        }

        
    }
}