using System.ComponentModel.DataAnnotations;

namespace Ceng382Week5.Models
{
    public class ClassInformationTable
    {
        public int Id { get; set; }  // Hidden but used for operations
        
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }
        
        [Display(Name = "Students")]
        public int StudentCount { get; set; }
        
        public string Description { get; set; }
    }
}