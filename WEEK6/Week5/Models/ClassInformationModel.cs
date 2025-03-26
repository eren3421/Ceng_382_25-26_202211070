// Models/ClassInformationModel.cs
using System.ComponentModel.DataAnnotations;

namespace Ceng382Week5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Class name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Class name must be between 2 and 100 characters")]
        public string ClassName { get; set; }
        
        [Required(ErrorMessage = "Student count is required")]
        [Range(1, 500, ErrorMessage = "Student count must be between 1 and 500")]
        public int StudentCount { get; set; }
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }
    }
}