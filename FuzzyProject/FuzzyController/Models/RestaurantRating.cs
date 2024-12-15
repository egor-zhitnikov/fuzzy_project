using System.ComponentModel.DataAnnotations;

namespace FuzzyController.Models
{
    public class RestaurantRating
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        [StringLength(100)] 
        public string RestaurantName { get; set; } 

        [Required]
        public string Rating { get; set; } 
    }
}