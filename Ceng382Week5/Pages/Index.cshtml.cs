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
        public List<ClassInformationModel> AllClasses => _classes; // Use full list of classes
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
            ApplyFiltersAndPagination(); // Apply filters and pagination for displaying

            // Only get class from full list, independent of filters, when editing
            if (EditId > 0)
            {
                ClassInfo = AllClasses.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
            }
        }

        private void ApplyFiltersAndPagination()
        {
            // Apply filters to the full class list for displaying in table
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

            // Apply pagination to the filtered data
            DisplayClasses = FilteredClasses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                //return RedirectToPage();
                ClassInfo.Id = _nextId++;
                _classes.Add(ClassInfo);
                TempData["Message"] = "Class added successfully!";
            // Re-apply filters and pagination after adding
                return RedirectToPage();
            }

            /// Add new class
                //ClassInfo.Id = _nextId++;
                //_classes.Add(ClassInfo);
                //TempData["Message"] = "Class added successfully!";

            // Re-apply filters and pagination after adding
                return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
           /* if (!ModelState.IsValid)
            {
                //ApplyFiltersAndPagination(); // Apply filters before rendering the page again
                return Page();
                
            }*/

            // Edit class using the full class list (ignoring filters)
            var existing = _classes.FirstOrDefault(c => c.Id == ClassInfo.Id);
            if (existing != null)
            {
                existing.ClassName = ClassInfo.ClassName;
                existing.StudentCount = ClassInfo.StudentCount;
                existing.Description = ClassInfo.Description;
                TempData["Message"] = "Class updated successfully!";
            }

            // Re-apply filters and pagination after editing
            ApplyFiltersAndPagination();
            return RedirectToPage(new { 
                currentPage = CurrentPage,
                filterClassName = FilterClassName,
                filterStudentCount = FilterStudentCount
            });
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = _classes.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                _classes.Remove(item);
                TempData["Message"] = "Class deleted successfully!";
            }

            // Re-apply filters and pagination after deletion
            ApplyFiltersAndPagination();
            return RedirectToPage(new { 
                currentPage = CurrentPage,
                filterClassName = FilterClassName,
                filterStudentCount = FilterStudentCount
            });
        }
    }
}
