using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LifeAssistant.Web.Migrations
{
    public partial class corrections2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ApplicationUserEntityId1",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserEntityId1",
                table: "Appointments",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ApplicationUserEntityId1",
                table: "Appointments",
                newName: "IX_Appointments_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ApplicationUserId",
                table: "Appointments",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_ApplicationUserId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Appointments",
                newName: "ApplicationUserEntityId1");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ApplicationUserId",
                table: "Appointments",
                newName: "IX_Appointments_ApplicationUserEntityId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_ApplicationUserEntityId1",
                table: "Appointments",
                column: "ApplicationUserEntityId1",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
