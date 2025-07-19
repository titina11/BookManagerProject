using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "PublisherId" },
                values: new object[] { "https://knigoman.bg/books/2303525764_1552613569786.png", 1 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "PublisherId" },
                values: new object[] { "https://cdn.ozone.bg/media/catalog/product/s/t/stakleniyat_tron_stakleniyat_tron_1_novo_izdanie_1713431567_0.jpg", 2 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageUrl", "PublisherId" },
                values: new object[] { "https://cdn.ozone.bg/media/catalog/product/i/g/igra-na-tronove-pesen-za-og-n-i-led-1.jpg", 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImageUrl",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "PublisherId" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "PublisherId" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageUrl", "PublisherId" },
                values: new object[] { null, 0 });
        }
    }
}
