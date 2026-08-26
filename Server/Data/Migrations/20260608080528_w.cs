using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class w : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_Images_PropertyId",
                table: "Images",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Properties_PropertyId",
                table: "Images",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Properties_PropertyId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_PropertyId",
                table: "Images");

            migrationBuilder.AddColumn<int>(
                name: "PropertiesPropertyID",
                table: "Images",
                type: "int",
                nullable: true);

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
    }
}
