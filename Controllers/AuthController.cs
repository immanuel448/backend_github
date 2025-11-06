// ------------------------------------------------------------
// AuthController.cs
// ------------------------------------------------------------
using backend_github.Data;
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
        public AuthController(AppDbContext context) => _context = context;

        public class ClaveRequest { public string clave { get; set; } = ""; }

        [HttpPost("verificar")]
        public IActionResult Verificar([FromBody] ClaveRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.clave))
                return BadRequest(new { acceso = false, mensaje = "Clave vacía" });

            using var sha = SHA256.Create();
            var hash = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(data.clave.Trim())))
                                   .Replace("-", "").ToLower();

            bool acceso = _context.Usuarios.Any(u => u.PasswordHash.ToLower() == hash);

            if (acceso)
                HttpContext.Session.SetString("autenticado", "true");

            return new JsonResult(new { acceso });
        }

        [HttpGet("verificarSesion")]
        public IActionResult VerificarSesion() =>
            new JsonResult(new { autenticado = HttpContext.Session.GetString("autenticado") == "true" });

        [HttpPost("salir")]
        public IActionResult Salir()
        {
            HttpContext.Session.Clear();
            return new JsonResult(new { mensaje = "Sesión cerrada" });
        }
    }
}
