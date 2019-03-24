using Microsoft.EntityFrameworkCore.Migrations;

namespace tthk.ContactsRegistry.Migrations
{
    public partial class ContactInitials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Initials",
                table: "Contacts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Initials",
                table: "Contacts");
        }
    }
}
