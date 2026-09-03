using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyIdToImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PropertiesPropertyID",
                table: "Images",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "Images",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Images_PropertiesPropertyID",
                table: "Images",
                column: "PropertiesPropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Properties_PropertiesPropertyID",
                table: "Images",
                column: "PropertiesPropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Properties_PropertiesPropertyID",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_PropertiesPropertyID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PropertiesPropertyID",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Images");
        }
    }
}
