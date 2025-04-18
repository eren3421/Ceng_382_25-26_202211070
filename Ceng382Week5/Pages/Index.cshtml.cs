using Ceng382Week5.Models;
using Ceng382Week5.Helpers;
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

        // Filtering
        [BindProperty(SupportsGet = true)]
        public string FilterClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterStudentCount { get; set; }

        // Pagination
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((decimal)FilteredClasses.Count / PageSize);

        // Form binding
        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        // Display Data
        public List<ClassInformationModel> AllClasses => _classes;
        public List<ClassInformationTable> FilteredClasses { get; set; }
        public List<ClassInformationTable> DisplayClasses { get; set; }

        public void OnGet()
        {
            ApplyFiltersAndPagination();

            if (EditId > 0)
            {
                ClassInfo = AllClasses.FirstOrDefault(c => c.Id == EditId) ?? new ClassInformationModel();
            }
        }
        //<!-- I created this code by can you add pagnition and filter function to my page -->
        private void ApplyFiltersAndPagination()
        {
            FilteredClasses = AllClasses
                .Where(c => string.IsNullOrEmpty(FilterClassName) || c.ClassName.Contains(FilterClassName, StringComparison.OrdinalIgnoreCase))
                .Where(c => !FilterStudentCount.HasValue || c.StudentCount == FilterStudentCount.Value)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                }).ToList();

            DisplayClasses = FilteredClasses
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                //return Page();
                ClassInfo.Id = _nextId++;
                _classes.Add(ClassInfo);
                TempData["Message"] = "Class added successfully!";
                return RedirectToPage();
            }

            /*ClassInfo.Id = _nextId++;
            _classes.Add(ClassInfo);
            TempData["Message"] = "Class added successfully!";*/
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var existing = _classes.FirstOrDefault(c => c.Id == ClassInfo.Id);
            if (existing != null)
            {
                existing.ClassName = ClassInfo.ClassName;
                existing.StudentCount = ClassInfo.StudentCount;
                existing.Description = ClassInfo.Description;
                TempData["Message"] = "Class updated successfully!";
            }

            return RedirectToPage(new
            {
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

            return RedirectToPage(new
            {
                currentPage = CurrentPage,
                filterClassName = FilterClassName,
                filterStudentCount = FilterStudentCount
            });
        }

        // ✅ JSON Export
        public JsonResult OnGetExportJson(bool filtered, string selectedColumns, string filterClassName, int? filterStudentCount, int currentPage = 1) //<!-- I created this by using chat gpt with can you add export function as json file and integrate to my project. -->
        {
            var selected = selectedColumns?.Split(',').ToList() ?? new List<string>();
            int pageSize = 10;

            var data = _classes
                .Where(c => !filtered || string.IsNullOrEmpty(filterClassName) || c.ClassName.Contains(filterClassName, StringComparison.OrdinalIgnoreCase))
                .Where(c => !filtered || !filterStudentCount.HasValue || c.StudentCount == filterStudentCount.Value)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description // I created the inside by using chat gpt with can you merge the export all and filtered export functions.
                })
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var json = Utils.Instance.ExportToJson(data, selected);
            return new JsonResult(json);
        }

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
    }
}
// I genreated the week 6.pdfs functionality by using chat gpt with uploading the pdf and asking can you update my index file to according to week 6.pdf.