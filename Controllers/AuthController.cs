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

        // 🔹 Clase para recibir el JSON del login
        public class LoginRequest
        {
            public string Correo { get; set; } = "";
            public string Password { get; set; } = "";
        }

        // ------------------------------------------------------------
        // POST: /api/auth/verificar
        // ------------------------------------------------------------
        [HttpPost("verificar")]
        public IActionResult Verificar([FromBody] LoginRequest data)
        {
            // 1️⃣ Busca el usuario en la base por correo
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Correo == data.Correo);

            if (usuario == null)
                return new JsonResult(new { acceso = false, mensaje = "Usuario no encontrado" });

            // 2️⃣ Calcula el hash de la contraseña enviada (en minúsculas)
            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(data.Password));
            var hashIngresado = BitConverter.ToString(hashBytes)
                                            .Replace("-", "")
                                            .ToLower(); // <-- forzamos minúsculas

            // 3️⃣ Compara con el hash guardado
            bool acceso = (hashIngresado == usuario.PasswordHash.ToLower());

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
