namespace backend_github.Models
{
    public class Usuario
    {
        public int Id { get; set; }                  // PK autoincremental
        public string Nombre { get; set; } = "";     // Nombre visible
        public string Correo { get; set; } = "";     // Usuario o email
        public string PasswordHash { get; set; } = ""; // Contraseña en hash
    }
}
