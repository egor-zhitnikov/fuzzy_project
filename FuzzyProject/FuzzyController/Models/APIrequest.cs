using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuzzyController.Models
{
    public class APIrequest
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Required data")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Required data")]
        public double FoodQuality { get; set; }

        [Required(ErrorMessage = "Required data")]
        public double ServiceLevel { get; set; }

        [Required(ErrorMessage = "Required data")]
        public double Atmosphere { get; set; }

        [Required(ErrorMessage = "Required data")]
        public string Comentary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Встановлення зовнішнього ключа
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }  // Навігаційна властивість
    }
}
