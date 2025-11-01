using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace backend_github.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        // ------------------------------------------------------------
        // Constructor: permite acceder a appsettings.json desde aquí
        // ------------------------------------------------------------
        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // ------------------------------------------------------------
        // POST: /api/auth/verificar
        // Recibe una contraseña, la compara con la correcta
        // y si coincide, guarda la sesión para recordar al usuario.
        // ------------------------------------------------------------
        [HttpPost("verificar")]
        public IActionResult Verificar([FromBody] dynamic data)
        {
            // 1. Obtiene la clave que el usuario escribió
            string claveIngresada = data?.clave;

            // 2. Lee la clave correcta desde appsettings.json
            string claveCorrecta = _config["AppSettings:ClaveAcceso"];

            // 3. Compara ambas
            bool acceso = (claveIngresada == claveCorrecta);

            // 4. Si es correcta, guarda un valor de sesión
            if (acceso)
            {
                // Crea una variable de sesión llamada "autenticado"
                HttpContext.Session.SetString("autenticado", "true");
            }

            // 5. Devuelve el resultado al frontend
            return Ok(new { acceso });
        }

        // ------------------------------------------------------------
        // GET: /api/auth/verificarSesion
        // Sirve para que el frontend sepa si el usuario ya inició sesión.
        // ------------------------------------------------------------
        [HttpGet("verificarSesion")]
        public IActionResult VerificarSesion()
        {
            // Intenta leer la variable de sesión "autenticado"
            var sesionActiva = HttpContext.Session.GetString("autenticado");

            // Si existe y vale "true", la sesión está activa
            bool autenticado = (sesionActiva == "true");

            return Ok(new { autenticado });
        }

        // ------------------------------------------------------------
        // POST: /api/auth/salir
        // Elimina la sesión actual (cerrar sesión)
        // ------------------------------------------------------------
        [HttpPost("salir")]
        public IActionResult Salir()
        {
            // Limpia toda la información de la sesión actual
            HttpContext.Session.Clear();

            // Devuelve confirmación al frontend
            return Ok(new { mensaje = "Sesión cerrada" });
        }
    }
}
