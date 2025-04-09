// Pages/Index.cshtml.cs
using Ceng382Week5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ceng382Week5.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> _classes = GenerateSampleData();
        private static int _nextId = _classes.Count + 1;

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string FilterClassName { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int? FilterStudentCount { get; set; }

        // Pagination properties
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(FilteredClasses.Count, PageSize));

        // Form binding
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        // Display properties
        public List<ClassInformationModel> AllClasses => _classes;
        public List<ClassInformationTable> FilteredClasses { get; set; }
        public List<ClassInformationTable> DisplayClasses { get; set; }

        private static List<ClassInformationModel> GenerateSampleData()
        {
            var random = new Random();
            return Enumerable.Range(1, 100).Select(i => new ClassInformationModel
            {
                Id = i,
                ClassName = $"Class {(i % 10) + 1}-{i}",
                StudentCount = random.Next(1, 100),
                Description = $"Sample class #{i}"
            }).ToList();
        }

        public void OnGet()
        {
            // Apply filters
            FilteredClasses = AllClasses
                .Where(c => string.IsNullOrEmpty(FilterClassName) || 
                            c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase))
                .Where(c => !FilterStudentCount.HasValue || 
                            c.StudentCount == FilterStudentCount.Value)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();

            // Apply pagination
            DisplayClasses = FilteredClasses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            if (EditId > 0)
            {
                ClassInfo = AllClasses.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
            }
        }

        public IActionResult OnPost()
        {
            if (Request.Form["handler"] == "Delete")
            {
                var id = int.Parse(Request.Form["id"]);
                var item = _classes.FirstOrDefault(c => c.Id == id);
                if (item != null) _classes.Remove(item);
                TempData["Message"] = "Class deleted successfully!";
                return RedirectToPage(new { 
                    currentPage = CurrentPage,
                    filterClassName = FilterClassName,
                    filterStudentCount = FilterStudentCount
                });
            }

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

            return RedirectToPage(new { 
                currentPage = CurrentPage,
                filterClassName = FilterClassName,
                filterStudentCount = FilterStudentCount
            });
        }
    }
}