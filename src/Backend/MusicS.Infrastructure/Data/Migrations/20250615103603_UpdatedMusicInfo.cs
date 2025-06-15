using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMusicInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Musics",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Musics");
        }
    }
}
