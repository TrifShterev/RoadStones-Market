using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace RoadStones_Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        [NotMapped]
        public string StreetAddress { get; set; }

        [NotMapped]
        public string City { get; set; }

        [NotMapped]
        public string PostalCode { get; set; }

        [NotMapped]
        public string Country { get; set; }
    }
}