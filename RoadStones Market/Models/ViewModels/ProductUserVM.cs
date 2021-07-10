using System.Collections.Generic;

namespace RoadStones_Market.Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductsList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }

        public IEnumerable<Product>  ProductsList { get; set; }
    }
}