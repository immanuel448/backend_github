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

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        // DTO que recibe el JSON { "clave": "..." } desde el frontend
        public class ClaveRequest
        {
            public string? clave { get; set; }
        }

        // POST: /api/auth/verificar
        // Recibe JSON y valida la contraseña
        [HttpPost("verificar")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult Verificar([FromBody] ClaveRequest data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.clave))
            {
                // Si no llegó JSON o faltó 'clave'
                return BadRequest(new { error = "Falta la propiedad 'clave' en el cuerpo JSON." });
            }

            string claveIngresada = data.clave.Trim();
            string claveCorrecta = _config["AppSettings:ClaveAcceso"] ?? "";

            bool acceso = (claveIngresada == claveCorrecta);

            if (acceso)
            {
                HttpContext.Session.SetString("autenticado", "true");
            }

            return new JsonResult(new { acceso });
        }

        // GET: /api/auth/verificarSesion
        [HttpGet("verificarSesion")]
        [Produces("application/json")]
        public IActionResult VerificarSesion()
        {
            var sesionActiva = HttpContext.Session.GetString("autenticado");
            bool autenticado = (sesionActiva == "true");
            return new JsonResult(new { autenticado });
        }

        // POST: /api/auth/salir
        [HttpPost("salir")]
        [Produces("application/json")]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return new JsonResult(new { mensaje = "Sesión cerrada" });
        }
    }
}
