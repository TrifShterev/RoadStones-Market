using Microsoft.AspNetCore.Identity;

namespace RoadStones_Market.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}