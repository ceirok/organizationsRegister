using Microsoft.EntityFrameworkCore.Migrations;

namespace tthk.ContactsRegistry.Migrations
{
    public partial class cpns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactPhoneNumber_Contacts_ContactId",
                table: "ContactPhoneNumber");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactPhoneNumber",
                table: "ContactPhoneNumber");

            migrationBuilder.RenameTable(
                name: "ContactPhoneNumber",
                newName: "ContactPhoneNumbers");

            migrationBuilder.RenameIndex(
                name: "IX_ContactPhoneNumber_ContactId",
                table: "ContactPhoneNumbers",
                newName: "IX_ContactPhoneNumbers_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactPhoneNumbers",
                table: "ContactPhoneNumbers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPhoneNumbers_Contacts_ContactId",
                table: "ContactPhoneNumbers",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactPhoneNumbers_Contacts_ContactId",
                table: "ContactPhoneNumbers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactPhoneNumbers",
                table: "ContactPhoneNumbers");

            migrationBuilder.RenameTable(
                name: "ContactPhoneNumbers",
                newName: "ContactPhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_ContactPhoneNumbers_ContactId",
                table: "ContactPhoneNumber",
                newName: "IX_ContactPhoneNumber_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactPhoneNumber",
                table: "ContactPhoneNumber",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactPhoneNumber_Contacts_ContactId",
                table: "ContactPhoneNumber",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
