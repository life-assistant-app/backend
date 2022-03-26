using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAssistant.Web.Migrations
{
    public partial class corrections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentEntity_Users_ApplicationUserEntityId",
                table: "AppointmentEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentEntity_Users_ApplicationUserEntityId1",
                table: "AppointmentEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentEntity",
                table: "AppointmentEntity");

            migrationBuilder.RenameTable(
                name: "AppointmentEntity",
                newName: "Appointments");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentEntity_ApplicationUserEntityId1",
                table: "Appointments",
                newName: "IX_Appointments_ApplicationUserEntityId1");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentEntity_ApplicationUserEntityId",
                table: "Appointments",
                newName: "IX_Appointments_ApplicationUserEntityId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "Appointments",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ApplicationUserEntityId",
                table: "Appointments",
                column: "ApplicationUserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ApplicationUserEntityId1",
                table: "Appointments",
                column: "ApplicationUserEntityId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ApplicationUserEntityId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ApplicationUserEntityId1",
                table: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Appointments",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "Appointments",
                newName: "AppointmentEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ApplicationUserEntityId1",
                table: "AppointmentEntity",
                newName: "IX_AppointmentEntity_ApplicationUserEntityId1");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ApplicationUserEntityId",
                table: "AppointmentEntity",
                newName: "IX_AppointmentEntity_ApplicationUserEntityId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "AppointmentEntity",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentEntity",
                table: "AppointmentEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentEntity_Users_ApplicationUserEntityId",
                table: "AppointmentEntity",
                column: "ApplicationUserEntityId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentEntity_Users_ApplicationUserEntityId1",
                table: "AppointmentEntity",
                column: "ApplicationUserEntityId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
