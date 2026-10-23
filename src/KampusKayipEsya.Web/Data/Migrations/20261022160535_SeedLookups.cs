using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KampusKayipEsya.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedLookups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Icon", "Name" },
                values: new object[,]
                {
                    { 1, "bi-phone", "Elektronik" },
                    { 2, "bi-person-vcard", "Kimlik/Kart" },
                    { 3, "bi-wallet2", "Cüzdan" },
                    { 4, "bi-key", "Anahtar" },
                    { 5, "bi-bag", "Giyim" },
                    { 6, "bi-book", "Kitap" },
                    { 7, "bi-three-dots", "Diğer" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Building", "FloorOrRoom", "Latitude", "Longitude" },
                values: new object[,]
                {
                    { 1, "Mühendislik Fakültesi", "Zemin kat", null, null },
                    { 2, "Mühendislik Fakültesi", "1. kat", null, null },
                    { 3, "Merkezi Derslik", "Amfi", null, null },
                    { 4, "Kütüphane", null, null, null },
                    { 5, "Yemekhane", null, null, null },
                    { 6, "Kantin", null, null, null },
                    { 7, "Spor Salonu", null, null, null },
                    { 8, "Güvenlik / Kayıp Eşya Bürosu", null, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
