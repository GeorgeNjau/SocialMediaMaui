using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SocialMediaMaui.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedPostEntity_RemovedPasswordProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Posts",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }
    }
}
