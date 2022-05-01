using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAssistant.Web.Migrations
{
    public partial class fk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ApplicationUserId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_ApplicationUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "LifeAssistantId",
                table: "Appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_LifeAssistantId",
                table: "Appointments",
                column: "LifeAssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_LifeAssistantId",
                table: "Appointments",
                column: "LifeAssistantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_LifeAssistantId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_LifeAssistantId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "LifeAssistantId",
                table: "Appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserId",
                table: "Appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ApplicationUserId",
                table: "Appointments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ApplicationUserId",
                table: "Appointments",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
