using backend_github.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1️⃣ Configurar SQLite y EF Core
// ------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=usuarios.db"));

// ------------------------------------------------------------
// 2️⃣ Controladores + Sesión
// ------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // 🔹 Permite cookies sin HTTPS (localhost)
    options.Cookie.SameSite = SameSiteMode.Lax;            // 🔹 Evita el rechazo del navegador
});

// ------------------------------------------------------------
// 3️⃣ CORS (para compatibilidad con front antiguo, pero ya no se usa Live Server)
// ------------------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirFrontend",
        policy =>
        {
            policy.WithOrigins(
                "http://127.0.0.1:5500",
                "http://localhost:5500"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// ------------------------------------------------------------
// 4️⃣ Middleware base
// ------------------------------------------------------------
app.UseStaticFiles();          // Sirve acceso.html, js, css, assets
app.UseRouting();
app.UseCors("PermitirFrontend");
app.UseSession();

// ------------------------------------------------------------
// 5️⃣ Middleware de seguridad (bloquea acceso sin sesión)
// ------------------------------------------------------------
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value?.ToLower();

    // Archivos siempre accesibles (sin login)
    if (path == "/" ||
        path.Contains("/api/") ||
        path.Contains("acceso.html") ||
        path.Contains(".js") ||
        path.Contains(".css") ||
        path.Contains(".png") ||
        path.Contains(".jpg") ||
        path.Contains(".webp") ||
        path.Contains(".ico") ||
        path.Contains(".mp3") ||
        path.Contains(".wav"))
    {
        await next();
        return;
    }

    // Verificar si hay sesión activa
    var sesion = context.Session.GetString("autenticado");

    if (sesion == "true")
    {
        // Evitar caché (para impedir ver al presionar “Atrás”)
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

// ------------------------------------------------------------
// 6️⃣ Rutas de la API
// ------------------------------------------------------------
app.MapControllers();

// ------------------------------------------------------------
// 7️⃣ Rutas de contenido protegido
// ------------------------------------------------------------

// Página principal (portada real)
app.MapGet("/index.html", async context =>
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

    await context.Response.SendFileAsync("PagesProtegidas/index.html");
});

// Capítulo 1
app.MapGet("/cap1.html", async context =>
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

    await context.Response.SendFileAsync("PagesProtegidas/cap1.html");
});

// Capítulo 2
app.MapGet("/cap2.html", async context =>
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

    await context.Response.SendFileAsync("PagesProtegidas/cap2.html");
});

// ------------------------------------------------------------
// 8️⃣ Ruta raíz (texto de diagnóstico opcional)
// ------------------------------------------------------------
app.MapGet("/", () => "Servidor con EFCore y SQLite funcionando correctamente 🔒");

app.Run();
