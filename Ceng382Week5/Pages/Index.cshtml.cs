using System.ComponentModel.DataAnnotations;
using Ceng382Week5.Models;
using Ceng382Week5.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ceng382Week5.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        // Filtering properties
        [BindProperty(SupportsGet = true)]
        public string FilterClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterStudentCount { get; set; }

        // Pagination properties
        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);

        // Form binding
        [BindProperty]
        public Class ClassInfo { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int EditId { get; set; }

        // Display Data
        public List<Class> DisplayClasses { get; set; }

        public async Task OnGetAsync()
        {
            if (!IsAuthenticated())
            {
                Response.Redirect("/Login");
                return;
            }

            // Uncomment if you want to populate the DB with fake data on first load
            // await GenerateSampleDataAsync();

            await ApplyFiltersAndPagination();

            if (EditId > 0)
            {
                ClassInfo = await _context.Classes.FirstOrDefaultAsync(c => c.Id == EditId) ?? new Class();
            }
        }

        private bool IsAuthenticated()
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieUsername = Request.Cookies["username"];
            var cookieToken = Request.Cookies["token"];
            var cookieSessionId = Request.Cookies["session_id"];

            return !string.IsNullOrEmpty(sessionUsername) &&
                   !string.IsNullOrEmpty(sessionToken) &&
                   !string.IsNullOrEmpty(sessionId) &&
                   sessionUsername == cookieUsername &&
                   sessionToken == cookieToken &&
                   sessionId == cookieSessionId;
        }

        private async Task ApplyFiltersAndPagination()
        {
            var query = _context.Classes.Where(c => c.IsActive);

            // Apply filters
            if (!string.IsNullOrEmpty(FilterClassName))
            {
                query = query.Where(c => c.Name.Contains(FilterClassName));
            }

            if (FilterStudentCount.HasValue)
            {
                query = query.Where(c => c.PersonCount == FilterStudentCount.Value);
            }

            // Get total count for pagination
            TotalItems = await query.CountAsync();

            // Apply pagination
            DisplayClasses = await query
                .OrderBy(c => c.Id)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                 ClassInfo.IsActive = true;
                _context.Classes.Add(ClassInfo);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Class added successfully!";
            return RedirectToPage();
            }

            /*_context.Classes.Add(ClassInfo);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Class added successfully!";*/
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var existing = await _context.Classes.FindAsync(ClassInfo.Id);
            if (existing != null)
            {
                existing.Name = ClassInfo.Name;
                existing.PersonCount = ClassInfo.PersonCount;
                existing.Description = ClassInfo.Description;
                existing.IsActive = true;

                await _context.SaveChangesAsync();
                TempData["Message"] = "Class updated successfully!";
            }

            return RedirectToPage(new
            {
                currentPage = CurrentPage,
                filterClassName = FilterClassName,
                filterStudentCount = FilterStudentCount
            });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                item.IsActive = false;
                //_context.Classes.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Class deleted successfully!";
            }

            return RedirectToPage(new
            {
                currentPage = CurrentPage,
                filterClassName = FilterClassName,
                filterStudentCount = FilterStudentCount
            });
        }

        public async Task<JsonResult> OnGetExportJsonAsync(bool filtered, string selectedColumns, string filterClassName, int? filterStudentCount, int currentPage = 1)
        {
           var query = _context.Classes
        .Where(c => c.IsActive) // 🔍 Yalnızca aktif olanlar
        .AsQueryable();

            if (filtered)
            {
                if (!string.IsNullOrEmpty(filterClassName))
                {
                    query = query.Where(c => c.Name.Contains(filterClassName));
                }

                if (filterStudentCount.HasValue)
                {
                    query = query.Where(c => c.PersonCount == filterStudentCount.Value);
                }
            }

            var data = await query
                .Skip((currentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            // Parse selectedColumns
            var selected = selectedColumns?.Split(',').ToList() ?? new List<string>();

            var export = data.Select(c =>
            {
                var dict = new Dictionary<string, object>();
                if (selected.Count == 0 || selected.Contains("Name")) dict["Name"] = c.Name;
                if (selected.Count == 0 || selected.Contains("PersonCount")) dict["PersonCount"] = c.PersonCount;
                if (selected.Count == 0 || selected.Contains("Description")) dict["Description"] = c.Description;
                return dict;
            });

            return new JsonResult(export);
        }

        // ✅ Sample data generator — sets IsActive to true
        public async Task GenerateSampleDataAsync()
        {
            if (!await _context.Classes.AnyAsync())
            {
                var rnd = new Random();
                var sampleClasses = Enumerable.Range(1, 100).Select(i => new Class
                {
                    Name = $"Class {(i % 10) + 1}-{i}",
                    PersonCount = rnd.Next(10, 100),
                    Description = $"Sample class #{i}",
                    IsActive = true
                });

                _context.Classes.AddRange(sampleClasses);
                await _context.SaveChangesAsync();
            }
        }
    }
}
