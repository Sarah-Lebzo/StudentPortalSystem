using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProject.Migrations
{
    /// <inheritdoc />
    public partial class EditAdminUserName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "StudentProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "179880fc-bcae-45be-b6b4-a41b872f366b", "AQAAAAIAAYagAAAAEG+3p7PelPk6jKaFKttmPPESrpqfWxH51H8WEjAS2J/Rfmed5SE8c9CnkcqcUKE0rg==", "4515600e-7e0e-4f7e-ae38-ed6e3319ad6d", "System Administrator" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "StudentProfiles");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "472ba632-6133-44a1-b158-6c10bd7d850d",
                columns: new[] { "ConcurrencyStamp", "FullName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "c3b46190-fe89-451b-bc4a-e588b15a922d", "System Administrator", "AQAAAAIAAYagAAAAEGCocipR/hWTihrLSAwjzh41C8meCfTEiZJqd1vl/8UJpeGLXnpZshHHMcn75cV4uQ==", "d94d75ed-6e00-426a-8394-39698b44a95b", "admin@gmail.com" });
        }
    }
}
