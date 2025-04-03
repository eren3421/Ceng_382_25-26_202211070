using Ceng382Week5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Ceng382Week5.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> Classes = new List<ClassInformationModel>();
        private static int nextId = 1;

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; }

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        public List<ClassInformationModel> ClassList => Classes;

        public void OnGet()
        {
            if (EditId > 0)
            {
                ClassInfo = Classes.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ClassInfo.Id = nextId++;
            Classes.Add(ClassInfo);
            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingClass = Classes.FirstOrDefault(c => c.Id == id);
            if (existingClass != null)
            {
                existingClass.ClassName = ClassInfo.ClassName;
                existingClass.StudentCount = ClassInfo.StudentCount;
                existingClass.Description = ClassInfo.Description;
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = Classes.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                Classes.Remove(classToDelete);
            }
            return RedirectToPage();
        }
    }
}