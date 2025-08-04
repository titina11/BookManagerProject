using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuthorCreatedUserIdAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("264a2a30-ec23-4aef-b1cb-8c7a4c9f7fa4"),
                column: "CreatedByUserId",
                value: "12345678-abcd-1234-abcd-1234567890ab");

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("9e340fd5-7f9e-43dc-96f0-07a3b9a1b12a"),
                column: "CreatedByUserId",
                value: "12345678-abcd-1234-abcd-1234567890ab");

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("fdddc2cb-718a-4cf3-9a8c-490c61cd31ae"),
                column: "CreatedByUserId",
                value: "12345678-abcd-1234-abcd-1234567890ab");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Authors");
        }
    }
}
