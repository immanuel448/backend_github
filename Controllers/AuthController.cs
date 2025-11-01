using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace backend_github.Controllers
{
    // ------------------------------------------------------------
    // [ApiController]: indica que esta clase manejará peticiones HTTP tipo API.
    // [Route("api/[controller]")]: define la ruta base. En este caso será:
    // http://localhost:puerto/api/auth
    // ------------------------------------------------------------
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        // ------------------------------------------------------------
        // Constructor: recibe la configuración del sistema (appsettings.json)
        // para poder leer la clave que guardamos ahí.
        // ------------------------------------------------------------
        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // ------------------------------------------------------------
        // POST: /api/auth/verificar
        // Este método recibe una contraseña desde el frontend (index.html),
        // la compara con la clave real del appsettings.json y devuelve si es válida.
        // ------------------------------------------------------------
        [HttpPost("verificar")]
        public IActionResult Verificar([FromBody] dynamic data)
        {
            // Extrae el valor "clave" del cuerpo de la petición JSON
            string claveIngresada = data?.clave;

            // Obtiene la clave correcta desde appsettings.json
            string claveCorrecta = _config["AppSettings:ClaveAcceso"];

            // Compara ambas claves (sensible a mayúsculas/minúsculas)
            bool acceso = (claveIngresada == claveCorrecta);

            // Devuelve la respuesta al frontend como JSON
            return Ok(new { acceso });
        }
    }
}
