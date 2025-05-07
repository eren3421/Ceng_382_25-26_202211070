using Microsoft.EntityFrameworkCore; // ekle
using Ceng382Week5.Data;            // DbContext için ekle

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// ✅ EF Core için DbContext servisini ekliyoruz:
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();       // eksikti, ekledin çok iyi!
app.UseRouting();

app.UseSession();           // MUTLAKA routing’den sonra gelmeli
app.UseAuthorization();

// Giriş yapmamış kullanıcıyı /Login'e yönlendir
app.Use(async (context, next) =>
{
    var path = context.Request.Path.ToString().ToLower();
    
    if (path == "/" || path == "/index")
    {
        var username = context.Session.GetString("username");
        if (string.IsNullOrEmpty(username))
        {
            context.Response.Redirect("/Login");
            return;
        }
    }
    await next();
});

app.MapRazorPages();

app.Run();
