using Microsoft.EntityFrameworkCore;
using RoadStones_Data.Data.Repository.IRepository;
using RoadStones_Models;

namespace RoadStones_Data.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        
        public CategoryRepository( ApplicationDbContext db) : base( db)
        {
            _db = db;
        }

        public void Update(Category obj)
        {
            //If we want to update by single property separately I can use this approach - otherwise --> see Update method in ProductRepository
            var objFromDb = base.FirstOrDefault(u => u.Id == obj.Id);

            if (objFromDb != null)
            {
                objFromDb.Name = obj.Name;
                objFromDb.DisplayOrder = obj.DisplayOrder;
            }
        }

        
    }
}