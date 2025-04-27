var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
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
app.UseStaticFiles(); // eksikse bunu da koy!
app.UseRouting();

app.UseSession();    // ✨ ROUTINGDEN SONRA MUTLAKA
app.UseAuthorization();

// BAŞTA GİRİŞ ZORUNLU İÇİN:
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
