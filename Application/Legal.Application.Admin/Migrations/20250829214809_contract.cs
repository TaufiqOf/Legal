using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legal.Application.Admin.Migrations
{
    /// <inheritdoc />
    public partial class contract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "admin");

            migrationBuilder.CreateTable(
                name: "contract",
                schema: "admin",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    author = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_modified_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    last_modified_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_owners_created_by",
                        column: x => x.created_by,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_contract_owners_last_modified_by",
                        column: x => x.last_modified_by,
                        principalSchema: "public",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contract_created_by",
                schema: "admin",
                table: "contract",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "ix_contract_last_modified_by",
                schema: "admin",
                table: "contract",
                column: "last_modified_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contract",
                schema: "admin");
        }
    }
}
