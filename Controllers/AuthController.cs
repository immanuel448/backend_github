using backend_github.Data;
using backend_github.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // 🔹 Clase para recibir la clave
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

            // Calcula el hash de la clave ingresada
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(claveIngresada));
            var hashIngresado = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            // Busca si existe algún usuario con ese hash
            bool acceso = _context.Usuarios.Any(u => u.PasswordHash.ToLower() == hashIngresado);

            if (acceso)
                HttpContext.Session.SetString("autenticado", "true");

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
