using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend_github.Migrations
{
    /// <inheritdoc />
    public partial class UsuariosEstaticos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Correo", "Nombre", "PasswordHash" },
                values: new object[,]
                {
                    { 1, "admin@historia.com", "Administrador", "5ac0852e770506dcd80f1a36d20ba7878bf82244b836d9324593bd14bc56dcb5" },
                    { 2, "invitado@historia.com", "Invitado", "06eda208dfb875dd849366a325a9e71e51df058731bf60a3b37f85ec6df12992" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
