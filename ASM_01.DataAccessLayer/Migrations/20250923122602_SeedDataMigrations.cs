using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASM_01.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Category",
                table: "Spec",
                type: "int",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "EvModel",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.InsertData(
                table: "EvModel",
                columns: new[] { "EvModelId", "Description", "ModelName", "Status" },
                values: new object[,]
                {
                    { 1, "Mid-size all-electric SUV", "VinFast VF8", 1 },
                    { 2, "Compact SUV with long range", "Tesla Model Y", 1 }
                });

            migrationBuilder.InsertData(
                table: "Spec",
                columns: new[] { "SpecId", "Category", "SpecName", "Unit" },
                values: new object[,]
                {
                    { 1, 0, "Battery Capacity", "kWh" },
                    { 2, 0, "Range", "km" },
                    { 3, 1, "Motor Power", "hp" },
                    { 4, 2, "Charging Time (fast)", "minutes" },
                    { 5, 3, "Seating Capacity", "seats" }
                });

            migrationBuilder.InsertData(
                table: "EvTrim",
                columns: new[] { "EvTrimId", "Description", "EvModelId", "ModelYear", "TrimName" },
                values: new object[,]
                {
                    { 1, "Entry version", 1, 2024, "VF8 Eco" },
                    { 2, "Premium version", 1, 2024, "VF8 Plus" },
                    { 3, "Dual motor", 2, 2024, "Model Y Long Range" },
                    { 4, "High performance", 2, 2024, "Model Y Performance" }
                });

            migrationBuilder.InsertData(
                table: "TrimPrice",
                columns: new[] { "TrimPriceId", "EffectiveDate", "EvTrimId", "ListedPrice" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 46000m },
                    { 2, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 47000m },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 52000m },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 55000m },
                    { 5, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 56000m },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 61000m }
                });

            migrationBuilder.InsertData(
                table: "TrimSpec",
                columns: new[] { "EvTrimId", "SpecId", "Value" },
                values: new object[,]
                {
                    { 1, 1, "82" },
                    { 1, 2, "420" },
                    { 1, 3, "350" },
                    { 1, 4, "35" },
                    { 1, 5, "5" },
                    { 2, 1, "87" },
                    { 2, 2, "470" },
                    { 2, 3, "402" },
                    { 2, 4, "30" },
                    { 2, 5, "5" },
                    { 3, 1, "82" },
                    { 3, 2, "497" },
                    { 3, 3, "384" },
                    { 3, 4, "27" },
                    { 3, 5, "5" },
                    { 4, 1, "82" },
                    { 4, 2, "450" },
                    { 4, 3, "456" },
                    { 4, 4, "25" },
                    { 4, 5, "5" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TrimPrice",
                keyColumn: "TrimPriceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TrimPrice",
                keyColumn: "TrimPriceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TrimPrice",
                keyColumn: "TrimPriceId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TrimPrice",
                keyColumn: "TrimPriceId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TrimPrice",
                keyColumn: "TrimPriceId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TrimPrice",
                keyColumn: "TrimPriceId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 1, 4 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 1, 5 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 2, 1 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 2, 5 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 3, 4 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 4, 1 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.DeleteData(
                table: "TrimSpec",
                keyColumns: new[] { "EvTrimId", "SpecId" },
                keyValues: new object[] { 4, 5 });

            migrationBuilder.DeleteData(
                table: "EvTrim",
                keyColumn: "EvTrimId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EvTrim",
                keyColumn: "EvTrimId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EvTrim",
                keyColumn: "EvTrimId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EvTrim",
                keyColumn: "EvTrimId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Spec",
                keyColumn: "SpecId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Spec",
                keyColumn: "SpecId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Spec",
                keyColumn: "SpecId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Spec",
                keyColumn: "SpecId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Spec",
                keyColumn: "SpecId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EvModel",
                keyColumn: "EvModelId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EvModel",
                keyColumn: "EvModelId",
                keyValue: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                table: "Spec",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EvModel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);
        }
    }
}
