using backend_github.Data;
using backend_github.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace backend_github.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Modelo para recibir el JSON
        public class ClaveRequest
        {
            public string clave { get; set; } = "";
        }

        // ------------------------------------------------------------
        // POST: /api/auth/verificar
        // ------------------------------------------------------------
        [HttpPost("verificar")]
        public IActionResult Verificar([FromBody] ClaveRequest data)
        {
            string claveIngresada = data.clave.Trim();

            // Calcular hash SHA256
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(claveIngresada));
            var hashIngresado = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            // Buscar si existe usuario con ese hash
            bool acceso = _context.Usuarios.Any(u => u.PasswordHash.ToLower() == hashIngresado);

            if (acceso)
            {
                // Guardar sesión
                HttpContext.Session.SetString("autenticado", "true");
            }

            return new JsonResult(new { acceso });
        }

        // ------------------------------------------------------------
        // GET: /api/auth/verificarSesion
        // ------------------------------------------------------------
        [HttpGet("verificarSesion")]
        public IActionResult VerificarSesion()
        {
            var sesionActiva = HttpContext.Session.GetString("autenticado");
            bool autenticado = (sesionActiva == "true");
            return new JsonResult(new { autenticado });
        }

        // ------------------------------------------------------------
        // POST: /api/auth/salir
        // ------------------------------------------------------------
        [HttpPost("salir")]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return new JsonResult(new { mensaje = "Sesión cerrada" });
        }
    }
}