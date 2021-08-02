using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RoadStones_Models.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }

        public IEnumerable<SelectListItem> CategorySelectList { get; set; }
    }
}