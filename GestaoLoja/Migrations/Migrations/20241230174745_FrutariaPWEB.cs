using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoLoja.Migrations
{
    /// <inheritdoc />
    public partial class FrutariaPWEB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "URLImagem",
                table: "Produtos",
                newName: "UrlImagem");

            migrationBuilder.RenameColumn(
                name: "URLImagem",
                table: "Categorias",
                newName: "UrlImagem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlImagem",
                table: "Produtos",
                newName: "URLImagem");

            migrationBuilder.RenameColumn(
                name: "UrlImagem",
                table: "Categorias",
                newName: "URLImagem");
        }
    }
}
