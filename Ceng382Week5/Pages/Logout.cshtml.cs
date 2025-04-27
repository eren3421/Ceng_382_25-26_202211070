using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Ceng382Week5.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Session'ı temizle
            HttpContext.Session.Clear();

            // Cookie'leri sil
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");

            // Login sayfasına yönlendir
            return RedirectToPage("/Login");
        }
    }
}
//I created this file by asking gpt can you create a logout page for my .net core project.