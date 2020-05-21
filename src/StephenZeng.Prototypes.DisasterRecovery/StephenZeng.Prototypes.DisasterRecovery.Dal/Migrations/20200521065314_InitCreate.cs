using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StephenZeng.Prototypes.DisasterRecovery.Dal.Migrations
{
    public partial class InitCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FriendlyId = table.Column<string>(maxLength: 16, nullable: true),
                    FirstName = table.Column<string>(maxLength: 256, nullable: true),
                    Surname = table.Column<string>(maxLength: 256, nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    MobilePhone = table.Column<string>(maxLength: 32, nullable: true),
                    Email = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectionBookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 36, nullable: true),
                    LastEventSequenceId = table.Column<long>(nullable: false),
                    LastChangedDate = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectionBookmarks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ProjectionBookmarks");
        }
    }
}
