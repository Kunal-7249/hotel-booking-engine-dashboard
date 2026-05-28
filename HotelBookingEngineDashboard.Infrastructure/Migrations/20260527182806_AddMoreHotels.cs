using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelBookingEngineDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreHotels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "HotelId", "City", "Description", "IsAvailable", "Name", "PricePerNight", "StarRating" },
                values: new object[,]
                {
                    { 7, "Chennai", "Iconic luxury hotel inspired by Chola architecture.", true, "ITC Grand Chola", 18000m, 5 },
                    { 8, "Pune", "Premium business hotel in Pune.", true, "Marriott", 9000m, 5 },
                    { 9, "Mumbai", "Luxury hotel with stunning city views.", true, "Hyatt Regency", 14000m, 5 },
                    { 10, "Delhi", "Modern hotel near business district.", true, "Radisson Blu", 7500m, 4 },
                    { 11, "Bangalore", "Comfortable stay in tech city.", true, "Fortune Select", 4500m, 4 },
                    { 12, "Udaipur", "Lakeside luxury with heritage charm.", true, "Trident Hotel", 20000m, 5 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Hotels",
                keyColumn: "HotelId",
                keyValue: 12);
        }
    }
}
