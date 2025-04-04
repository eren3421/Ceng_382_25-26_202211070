using Ceng382Week5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Ceng382Week5.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> _classes = new();
        private static int _nextId = 1;

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        public List<ClassInformationModel> ClassList => _classes;

        public void OnGet()
        {
            if (EditId > 0)
            {
                ClassInfo = _classes.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
            }
        }

        public IActionResult OnPost()
        {
            // Handle Delete first (doesn't need validation)
            if (Request.Form["handler"] == "Delete")
            {
                var id = int.Parse(Request.Form["id"]);
                var item = _classes.FirstOrDefault(c => c.Id == id);
                if (item != null) _classes.Remove(item);
                TempData["Message"] = "Class deleted successfully!";
                return RedirectToPage();
            }

            // Validate for Add/Edit operations
            if (!ModelState.IsValid)
            {
                return Page();
            }

            switch (Request.Form["handler"])
            {
                case "Add":
                    ClassInfo.Id = _nextId++;
                    _classes.Add(ClassInfo);
                    TempData["Message"] = "Class added successfully!";
                    break;
                    
                case "Edit":
                    var existing = _classes.FirstOrDefault(c => c.Id == ClassInfo.Id);
                    if (existing != null)
                    {
                        existing.ClassName = ClassInfo.ClassName;
                        existing.StudentCount = ClassInfo.StudentCount;
                        existing.Description = ClassInfo.Description;
                        TempData["Message"] = "Class updated successfully!";
                    }
                    break;
            }

            return RedirectToPage();
        }
    }
}