using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Habilitar controladores y sesiones
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔹 Permitir servir archivos estáticos (HTML, CSS, JS en wwwroot)
builder.Services.AddRouting();

var app = builder.Build();

// 🔹 Middleware básico
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllers();

// 🔹 Ruta base (solo para comprobar)
app.MapGet("/", () => "Servidor backend funcionando correctamente ✅");

app.Run();
