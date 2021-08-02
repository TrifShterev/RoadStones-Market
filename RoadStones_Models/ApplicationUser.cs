using Microsoft.AspNetCore.Identity;

namespace RoadStones_Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}