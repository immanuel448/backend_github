# 🛡️ backend_github (.NET)

**Sistema de autenticación seguro en ASP.NET Core + SQLite**, diseñado para manejar acceso por contraseña, sesiones persistentes y protección de páginas privadas.  

Desarrollado como backend independiente del proyecto **“Una Historia Simple?”**, pero adaptable a cualquier aplicación que requiera control de acceso por sesión.

---

## ⚙️ CARACTERÍSTICAS PRINCIPALES

- Inicio de sesión mediante contraseñas **hasheadas (SHA256)**.  
- Base de datos local **SQLite** gestionada con **Entity Framework Core**.  
- **Sesión persistente** (no pide contraseña al cambiar de página).  
- **Middleware** que bloquea el acceso sin autenticación.  
- **Prevención de caché** al presionar “Atrás” tras cerrar sesión.  
- Listo para integrarse con cualquier **frontend (HTML, JS, etc.)**.  
- Ideal para despliegue en **Render, Railway o servidores locales**.

---

## 📂 ESTRUCTURA DEL PROYECTO
```text
backend_github/
│
├── Controllers/
│   └── AuthController.cs          # Controlador principal de autenticación
│
├── Data/
│   └── AppDbContext.cs            # Configuración de EF Core + datos iniciales
│
├── Models/
│   └── Usuario.cs                 # Entidad base (Id, Nombre, Correo, Hash)
│
├── Migrations/                    # Archivos generados por EF Core
│
├── PagesProtegidas/               # Archivos HTML protegidos (ejemplo)
│   ├── index.html
│   ├── cap1.html
│   └── cap2.html
│
├── wwwroot/                       # Contenido público (login, JS, CSS, imágenes)
│   ├── acceso.html
│   ├── js/
│   ├── css/
│   └── assets/
│
├── usuarios.db                    # Base de datos SQLite local
│
├── Program.cs                     # Configuración del servidor y middleware
│
└── README.md


💡 FUNCIONAMIENTO

1️⃣ Inicio de sesión (AuthController.cs)

El usuario envía su contraseña vía POST /api/auth/verificar.

Se genera un hash SHA256 y se compara con los registros en usuarios.db.

Si coincide:
HttpContext.Session.SetString("autenticado", "true");

Luego el middleware permite el acceso a las páginas protegidas.

2️⃣ Protección de rutas (Program.cs)
Cada solicitud pasa por un middleware que verifica la sesión:

app.Use(async (context, next) =>
{
var path = context.Request.Path.Value?.ToLower();
if (path == "/" ||
    path.Contains("/api/") ||
    path.Contains("acceso.html") ||
    path.Contains(".js") ||
    path.Contains(".css"))
{
    await next();
    return;
}

var sesion = context.Session.GetString("autenticado");
if (sesion == "true")
{
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    await next();
}
else
{
    context.Response.Redirect("/acceso.html");
}

if (path == "/" ||
    path.Contains("/api/") ||
    path.Contains("acceso.html") ||
    path.Contains(".js") ||
    path.Contains(".css"))
{
    await next();
    return;
}

var sesion = context.Session.GetString("autenticado");
if (sesion == "true")
{
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    await next();
}
else
{
    context.Response.Redirect("/acceso.html");
}

