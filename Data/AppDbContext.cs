// ------------------------------------------------------------
// AppDbContext.cs
// ------------------------------------------------------------
using backend_github.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace backend_github.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            static string Hash(string input)
            {
                using var sha = SHA256.Create();
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(input)))
                                   .Replace("-", "").ToLower();
            }

            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { Id = 1, Nombre = "Administrador", Correo = "admin@historia.com", PasswordHash = Hash("clave123") },
                new Usuario { Id = 2, Nombre = "Invitado", Correo = "invitado@historia.com", PasswordHash = Hash("hola2025") }
            );
        }
    }
}