using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using RoadStones_Models;

namespace RoadStones_Data.Data.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

        IEnumerable<SelectListItem> GetAllDropdownItems(string obj);
    }
}