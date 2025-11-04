using backend_github.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Crear builder
var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 1️⃣ Configurar EF Core con SQLite
// ------------------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=usuarios.db"));

// ------------------------------------------------------------
// 2️⃣ Controladores + Sesiones
// ------------------------------------------------------------
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // 🔹 Permite cookies sin HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax;            // 🔹 Evita el rechazo en localhost
});

builder.Services.AddRouting();

// ------------------------------------------------------------
// 3️⃣ CORS (permitir al frontend acceder al backend)
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
            .AllowCredentials(); // 🔹 Necesario para cookies de sesión
        });
});

// ------------------------------------------------------------
// 4️⃣ Construcción del app
// ------------------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();

// ------------------------------------------------------------
// 5️⃣ Middleware
// ------------------------------------------------------------
app.UseStaticFiles();
app.UseRouting();
app.UseCors("PermitirFrontend");
app.UseSession(); // 🔹 Importante: antes de MapControllers
app.MapControllers();

// ------------------------------------------------------------
// 6️⃣ Ruta de prueba
// ------------------------------------------------------------
app.MapGet("/", () => "Servidor con EFCore y SQLite funcionando");

app.Run();