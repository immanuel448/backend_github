// ------------------------------------------------------------
// Program.cs
// ------------------------------------------------------------
using backend_github.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Base de datos SQLite + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=usuarios.db"));

// 2️⃣ Controladores + Sesiones
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Permite sin HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 3️⃣ CORS (solo si alguna vez lo usas con otro frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// 4️⃣ Middleware base
app.UseStaticFiles(); // Sirve acceso.html, css, js, assets
app.UseRouting();
app.UseCors("PermitirFrontend");
app.UseSession();

// 5️⃣ Middleware de protección general
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Archivos y rutas públicas
    if (path == "/" ||
        path.Contains("/api/") ||
        path.Contains("acceso.html") ||
        path.EndsWith(".js") ||
        path.EndsWith(".css") ||
        path.EndsWith(".png") ||
        path.EndsWith(".jpg") ||
        path.EndsWith(".webp") ||
        path.EndsWith(".ico") ||
        path.EndsWith(".mp3") ||
        path.EndsWith(".wav"))
    {
        await next();
        return;
    }

    // Verificación de sesión
    var sesion = context.Session.GetString("autenticado");
    if (sesion == "true")
    {
        // Evita contenido cacheado (impide volver atrás)
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        await next();
    }
    else
    {
        context.Response.Redirect("/acceso.html");
    }
});

// 6️⃣ Controladores de API
app.MapControllers();

// 7️⃣ Rutas protegidas (HTML de PagesProtegidas)
string paginasProtegidas = Path.Combine(app.Environment.ContentRootPath, "PagesProtegidas");

async Task ProtegerPagina(HttpContext context, string archivo)
{
    var sesion = context.Session.GetString("autenticado");
    if (sesion != "true")
    {
        context.Response.Redirect("/acceso.html");
        return;
    }

    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "0";

    await context.Response.SendFileAsync(Path.Combine(paginasProtegidas, archivo));
}

app.MapGet("/index.html", ctx => ProtegerPagina(ctx, "index.html"));
app.MapGet("/cap1.html", ctx => ProtegerPagina(ctx, "cap1.html"));
app.MapGet("/cap2.html", ctx => ProtegerPagina(ctx, "cap2.html"));

// 8️⃣ Ruta raíz
app.MapGet("/", () => "Servidor con EFCore y SQLite funcionando correctamente");

app.Run();