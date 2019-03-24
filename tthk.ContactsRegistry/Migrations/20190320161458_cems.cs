using Microsoft.EntityFrameworkCore.Migrations;

namespace tthk.ContactsRegistry.Migrations
{
    public partial class cems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactEmail_Contacts_ContactId",
                table: "ContactEmail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactEmail",
                table: "ContactEmail");

            migrationBuilder.RenameTable(
                name: "ContactEmail",
                newName: "ContactEmails");

            migrationBuilder.RenameIndex(
                name: "IX_ContactEmail_ContactId",
                table: "ContactEmails",
                newName: "IX_ContactEmails_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactEmails",
                table: "ContactEmails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactEmails_Contacts_ContactId",
                table: "ContactEmails",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactEmails_Contacts_ContactId",
                table: "ContactEmails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactEmails",
                table: "ContactEmails");

            migrationBuilder.RenameTable(
                name: "ContactEmails",
                newName: "ContactEmail");

            migrationBuilder.RenameIndex(
                name: "IX_ContactEmails_ContactId",
                table: "ContactEmail",
                newName: "IX_ContactEmail_ContactId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactEmail",
                table: "ContactEmail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactEmail_Contacts_ContactId",
                table: "ContactEmail",
                column: "ContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
