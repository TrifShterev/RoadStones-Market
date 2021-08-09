using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoadStones_Models
{
    public class Product
    {
        public Product()
        {
            TempSqMeters = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        [Range(0,double.MaxValue)]
        public double Price { get; set; }

        public string Image { get; set; }

        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [NotMapped]
        [Range(1,1000)]
        public double TempSqMeters { get; set; }
    }
}