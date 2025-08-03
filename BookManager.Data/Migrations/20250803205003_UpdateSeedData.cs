using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Reviews",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "12345678-abcd-1234-abcd-1234567890ab",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEG3fsRf5GXhdPIBCvggI4+9nD0u41qgonyEWk6/efTN14w0wR4iRD2ESwZyfk3dRrA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "12345678-abcd-1234-abcd-1234567890ab",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFpUyPwhlTG4HOSov1cF6OWg4+jE7ZuOw2TCTmUyU/OSGfPoTOE48qMJ/VoSxSPmbw==");
        }
    }
}
