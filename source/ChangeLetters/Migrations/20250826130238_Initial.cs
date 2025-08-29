using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChangeLetters.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VocabularyItems",
                columns: table => new
                {
                    VocabularyItemId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UnknownWord = table.Column<string>(type: "TEXT", nullable: false),
                    CorrectedWord = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VocabularyItems", x => x.VocabularyItemId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VocabularyItems");
        }
    }
}
