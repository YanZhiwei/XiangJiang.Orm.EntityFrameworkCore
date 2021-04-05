using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace XiangJiang.Orm.EntityFrameworkCoreTests.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EFSample",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CreateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifyTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Available = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EFSample", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EFSample");
        }
    }
}
