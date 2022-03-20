using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAssistant.Web.Migrations
{
    public partial class appointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApplicationUserEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApplicationUserEntityId1 = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentEntity_Users_ApplicationUserEntityId",
                        column: x => x.ApplicationUserEntityId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppointmentEntity_Users_ApplicationUserEntityId1",
                        column: x => x.ApplicationUserEntityId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentEntity_ApplicationUserEntityId",
                table: "AppointmentEntity",
                column: "ApplicationUserEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentEntity_ApplicationUserEntityId1",
                table: "AppointmentEntity",
                column: "ApplicationUserEntityId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentEntity");
        }
    }
}
