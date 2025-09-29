using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASM_01.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddWarehouseManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dealers",
                columns: table => new
                {
                    DealerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dealers", x => x.DealerId);
                });

            migrationBuilder.CreateTable(
                name: "DistributionRequests",
                columns: table => new
                {
                    DistributionRequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    EvTrimId = table.Column<int>(type: "int", nullable: false),
                    RequestedQuantity = table.Column<int>(type: "int", nullable: false),
                    ApprovedQuantity = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistributionRequests", x => x.DistributionRequestId);
                    table.ForeignKey(
                        name: "FK_DistributionRequests_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "DealerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DistributionRequests_EvTrim_EvTrimId",
                        column: x => x.EvTrimId,
                        principalTable: "EvTrim",
                        principalColumn: "EvTrimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleStocks",
                columns: table => new
                {
                    VehicleStockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DealerId = table.Column<int>(type: "int", nullable: false),
                    EvTrimId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleStocks", x => x.VehicleStockId);
                    table.ForeignKey(
                        name: "FK_VehicleStocks_Dealers_DealerId",
                        column: x => x.DealerId,
                        principalTable: "Dealers",
                        principalColumn: "DealerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleStocks_EvTrim_EvTrimId",
                        column: x => x.EvTrimId,
                        principalTable: "EvTrim",
                        principalColumn: "EvTrimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Dealers",
                columns: new[] { "DealerId", "Address", "Name" },
                values: new object[] { 1, "New York, NY", "City EV Motors" });

            migrationBuilder.InsertData(
                table: "VehicleStocks",
                columns: new[] { "VehicleStockId", "DealerId", "EvTrimId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 1, 5 },
                    { 2, 1, 3, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistributionRequests_DealerId",
                table: "DistributionRequests",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_DistributionRequests_EvTrimId",
                table: "DistributionRequests",
                column: "EvTrimId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleStocks_DealerId",
                table: "VehicleStocks",
                column: "DealerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleStocks_EvTrimId",
                table: "VehicleStocks",
                column: "EvTrimId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistributionRequests");

            migrationBuilder.DropTable(
                name: "VehicleStocks");

            migrationBuilder.DropTable(
                name: "Dealers");
        }
    }
}
