using System.ComponentModel.DataAnnotations;

namespace Ceng382Week5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Class name is required")]
        public string ClassName { get; set; }
        
        [Required(ErrorMessage = "Student count is required")]
        [Range(1, 100, ErrorMessage = "Student count must be between 1 and 100")]
        public int StudentCount { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}