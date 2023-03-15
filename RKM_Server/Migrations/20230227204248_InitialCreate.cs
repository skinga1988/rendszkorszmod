using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RKM_Server.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemPrice = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowId = table.Column<int>(type: "int", nullable: false),
                    ColumnId = table.Column<int>(type: "int", nullable: false),
                    BoxId = table.Column<int>(type: "int", nullable: false),
                    MaxPieces = table.Column<int>(type: "int", nullable: false),
                    AvailablePieces = table.Column<int>(type: "int", nullable: false),
                    ReservedPieces = table.Column<int>(type: "int", nullable: false),
                    StockItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_StockItems_StockItemId",
                        column: x => x.StockItemId,
                        principalTable: "StockItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orderers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdererName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orderers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orderers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
               name: "Projects",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable: false),
                   UserId = table.Column<int>(type: "int", nullable: false),
                   Type = table.Column<int>(type: "int", nullable: false),
                   ProjectDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   Place = table.Column<string>(type: "nvarchar(max)", nullable: false),
                   OrdererId = table.Column<int>(type: "int", nullable: false),

               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_Projects", x => new { x.Id });
                   table.ForeignKey(
                       name: "FK_Projects_Orderers_OrdererId",
                       column: x => x.OrdererId,
                       principalTable: "Orderers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                   table.ForeignKey(
                       name: "FK_Projects_Users_UserId",
                       column: x => x.UserId,
                       principalTable: "Users",
                       principalColumn: "UserId",
                       onDelete: ReferentialAction.Restrict);
               });

            migrationBuilder.CreateTable(
                name: "ProjectAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_ProjectAccounts_Projects_ProjectId",
                         column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id");
                });

           

            migrationBuilder.CreateTable(
                name: "StockAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Pieces = table.Column<int>(type: "int", nullable: false),
                    AccountTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    StockItemId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                table.PrimaryKey("PK_StockAccounts", x => new { x.Id });
                table.ForeignKey(
                    name: "FK_StockAccounts_Projects_ProjectId",
                    columns: x => new { x.ProjectId },
                    principalTable: "Projects",
                    principalColumns: new[] { "Id" },
                    onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAccounts_StockItems_StockItemId",
                        column: x => x.StockItemId,
                        principalTable: "StockItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockAccounts_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orderers_UserId",
                table: "Orderers",
                column: "UserId");


            migrationBuilder.CreateIndex(
                name: "IX_ProjectAccounts_UserId",
                table: "ProjectAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrdererId",
                table: "Projects",
                column: "OrdererId",
                unique: true);


            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserId",
                table: "Projects",
                column: "UserId");


            migrationBuilder.CreateIndex(
                name: "IX_StockAccounts_StockId",
                table: "StockAccounts",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAccounts_StockItemId",
                table: "StockAccounts",
                column: "StockItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAccounts_UserId",
                table: "StockAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_StockItemId",
                table: "Stocks",
                column: "StockItemId");



            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orderers_Users_UserId",
                table: "Orderers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UserId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAccounts_Users_UserId",
                table: "StockAccounts");


            migrationBuilder.DropTable(
                name: "ProjectAccounts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Orderers");

            migrationBuilder.DropTable(
                name: "StockAccounts");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "StockItems");
        }
    }
}
