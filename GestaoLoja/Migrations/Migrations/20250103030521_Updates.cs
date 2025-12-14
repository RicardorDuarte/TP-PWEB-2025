using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoLoja.Migrations
{
    /// <inheritdoc />
    public partial class Updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Localidade2",
                table: "AspNetUsers",
                newName: "Localidade");

            migrationBuilder.RenameColumn(
                name: "Localidade1",
                table: "AspNetUsers",
                newName: "Estado");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Localidade",
                table: "AspNetUsers",
                newName: "Localidade2");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "AspNetUsers",
                newName: "Localidade1");
        }
    }
}
