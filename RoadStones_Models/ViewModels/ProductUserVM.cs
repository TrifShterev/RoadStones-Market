using System.Collections.Generic;

namespace RoadStones_Models.ViewModels
{
    public class ProductUserVM
    {
        public ProductUserVM()
        {
            ProductsList = new List<Product>();
        }

        public ApplicationUser ApplicationUser { get; set; }

        public List<Product>  ProductsList { get; set; }
    }
}