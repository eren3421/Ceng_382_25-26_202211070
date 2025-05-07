using System.ComponentModel.DataAnnotations;

namespace Ceng382Week5.Models
{
    public class Class
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Student count is required")]
        [Range(1, 100, ErrorMessage = "Student count must be between 1 and 100")]
        public int PersonCount { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public bool IsActive { get; set; }
    }
}
