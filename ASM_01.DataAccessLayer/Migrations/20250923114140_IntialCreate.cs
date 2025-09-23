using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASM_01.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvModel",
                columns: table => new
                {
                    EvModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvModel", x => x.EvModelId);
                });

            migrationBuilder.CreateTable(
                name: "Spec",
                columns: table => new
                {
                    SpecId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Spec", x => x.SpecId);
                });

            migrationBuilder.CreateTable(
                name: "EvTrim",
                columns: table => new
                {
                    EvTrimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvModelId = table.Column<int>(type: "int", nullable: false),
                    TrimName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ModelYear = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvTrim", x => x.EvTrimId);
                    table.ForeignKey(
                        name: "FK_EvTrim_EvModel_EvModelId",
                        column: x => x.EvModelId,
                        principalTable: "EvModel",
                        principalColumn: "EvModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrimPrice",
                columns: table => new
                {
                    TrimPriceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvTrimId = table.Column<int>(type: "int", nullable: false),
                    ListedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrimPrice", x => x.TrimPriceId);
                    table.ForeignKey(
                        name: "FK_TrimPrice_EvTrim_EvTrimId",
                        column: x => x.EvTrimId,
                        principalTable: "EvTrim",
                        principalColumn: "EvTrimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrimSpec",
                columns: table => new
                {
                    EvTrimId = table.Column<int>(type: "int", nullable: false),
                    SpecId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrimSpec", x => new { x.EvTrimId, x.SpecId });
                    table.ForeignKey(
                        name: "FK_TrimSpec_EvTrim_EvTrimId",
                        column: x => x.EvTrimId,
                        principalTable: "EvTrim",
                        principalColumn: "EvTrimId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrimSpec_Spec_SpecId",
                        column: x => x.SpecId,
                        principalTable: "Spec",
                        principalColumn: "SpecId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvTrim_EvModelId",
                table: "EvTrim",
                column: "EvModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Spec_SpecName",
                table: "Spec",
                column: "SpecName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrimPrice_EvTrimId",
                table: "TrimPrice",
                column: "EvTrimId");

            migrationBuilder.CreateIndex(
                name: "IX_TrimSpec_SpecId",
                table: "TrimSpec",
                column: "SpecId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrimPrice");

            migrationBuilder.DropTable(
                name: "TrimSpec");

            migrationBuilder.DropTable(
                name: "EvTrim");

            migrationBuilder.DropTable(
                name: "Spec");

            migrationBuilder.DropTable(
                name: "EvModel");
        }
    }
}
