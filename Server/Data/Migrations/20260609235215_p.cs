using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class p : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_Properties_PropertiesPropertyID",
                table: "Amenities");

            migrationBuilder.RenameColumn(
                name: "ReviewId",
                table: "Reviews",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PropertyID",
                table: "Properties",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "OwnerID",
                table: "Owners",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ImageId",
                table: "Images",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PropertiesPropertyID",
                table: "Amenities",
                newName: "PropertiesId");

            migrationBuilder.RenameColumn(
                name: "AmenityID",
                table: "Amenities",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Amenities_PropertiesPropertyID",
                table: "Amenities",
                newName: "IX_Amenities_PropertiesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_Properties_PropertiesId",
                table: "Amenities",
                column: "PropertiesId",
                principalTable: "Properties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Amenities_Properties_PropertiesId",
                table: "Amenities");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Reviews",
                newName: "ReviewId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Properties",
                newName: "PropertyID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Owners",
                newName: "OwnerID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Images",
                newName: "ImageId");

            migrationBuilder.RenameColumn(
                name: "PropertiesId",
                table: "Amenities",
                newName: "PropertiesPropertyID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Amenities",
                newName: "AmenityID");

            migrationBuilder.RenameIndex(
                name: "IX_Amenities_PropertiesId",
                table: "Amenities",
                newName: "IX_Amenities_PropertiesPropertyID");

            migrationBuilder.AddForeignKey(
                name: "FK_Amenities_Properties_PropertiesPropertyID",
                table: "Amenities",
                column: "PropertiesPropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID");
        }
    }
}
