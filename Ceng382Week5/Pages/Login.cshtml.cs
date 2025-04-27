using Ceng382Week5.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace Ceng382Week5.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnPost()
        {
/*if (!ModelState.IsValid)
    {
        ErrorMessage = "Lütfen tüm alanları doldurun.";
        return Page();
    }*/
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
            var json = System.IO.File.ReadAllText(path);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var user = users.FirstOrDefault(u =>
                u.Username == Username &&
                u.Password == Password &&
                u.IsActive);

            if (user == null)
            {
                ErrorMessage = "Geçersiz kullanıcı adı veya şifre veya kullanıcı aktif değil.";
                return Page();
            }

            // Token üret
            var token = Guid.NewGuid().ToString();

            // Session’a yaz
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // Cookie’ye yaz
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", user.Username, options);
            Response.Cookies.Append("token", token, options);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, options);

            // Başarılı giriş → Index'e yönlendir
            return RedirectToPage("/Index");
        }
    }
}// I created this page by asking gpt can you write a login.cshtml.cs page for my .net core project. 