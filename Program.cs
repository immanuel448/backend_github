using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ------------------------------------------------------------
// 1. Crear el generador de la aplicación web (WebApplicationBuilder)
// ------------------------------------------------------------
// Este objeto configura los servicios (controladores, sesiones, etc.)
// y luego construye la aplicación antes de ejecutarla.
var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// 2. Registrar servicios en el contenedor de dependencias
// ------------------------------------------------------------

// Habilita el uso de controladores (para manejar rutas tipo /api/...).
builder.Services.AddControllers();

// Activa una memoria caché en RAM para almacenar sesiones temporalmente.
builder.Services.AddDistributedMemoryCache();

// Habilita el manejo de sesiones en la aplicación.
// La sesión permite recordar si el usuario ya inició sesión
// o ingresó la contraseña correcta, sin pedírsela cada vez.
builder.Services.AddSession(options =>
{
    // Tiempo máximo de inactividad antes de cerrar la sesión (30 minutos)
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Hace que la cookie de sesión solo pueda ser leída por el servidor
    options.Cookie.HttpOnly = true;

    // Indica que esta cookie es esencial para el funcionamiento del sitio
    options.Cookie.IsEssential = true;
});

// Habilita el sistema de enrutamiento, necesario para mapear URLs a controladores.
builder.Services.AddRouting();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// ------------------------------------------------------------
// 3. Construir la aplicación con la configuración anterior
// ------------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------------
// 4. Configurar los middlewares (componentes intermedios del servidor)
// ------------------------------------------------------------

// Si estamos en modo desarrollo, mostrar información detallada de errores.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

// Habilita el uso de archivos estáticos (como HTML, CSS, JS, imágenes)
// desde la carpeta wwwroot.
app.UseStaticFiles();

// Activa el sistema de enrutamiento.
app.UseRouting();

// Activa el soporte de sesiones (debe ir después del enrutamiento).
app.UseSession();

// Asocia los controladores a las rutas disponibles (por ejemplo: /api/auth/...).
app.MapControllers();

// ------------------------------------------------------------
// 5. Ruta básica de prueba
// ------------------------------------------------------------
// Esta ruta responde cuando visitas la raíz del sitio (http://localhost:puerto/).
// Solo sirve para verificar que el servidor está funcionando correctamente.
app.MapGet("/", () => "Servidor backend funcionando correctamente");

// ------------------------------------------------------------
// 6. Ejecutar la aplicación (bloquea el hilo principal hasta que se detenga)
// ------------------------------------------------------------
app.Run();
