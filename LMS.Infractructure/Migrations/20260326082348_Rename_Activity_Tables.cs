using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Activity_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivities_ModuleActivityTypes_ActivityTypeId",
                table: "ModuleActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivities_Modules_ModuleId",
                table: "ModuleActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleActivityTypes",
                table: "ModuleActivityTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModuleActivities",
                table: "ModuleActivities");

            migrationBuilder.RenameTable(
                name: "ModuleActivityTypes",
                newName: "ActivityTypes");

            migrationBuilder.RenameTable(
                name: "ModuleActivities",
                newName: "Activities");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleActivities_ModuleId",
                table: "Activities",
                newName: "IX_Activities_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleActivities_ActivityTypeId",
                table: "Activities",
                newName: "IX_Activities_ActivityTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityTypes",
                table: "ActivityTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Activities",
                table: "Activities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_ActivityTypes_ActivityTypeId",
                table: "Activities",
                column: "ActivityTypeId",
                principalTable: "ActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Activities_Modules_ModuleId",
                table: "Activities",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Activities_ActivityTypes_ActivityTypeId",
                table: "Activities");

            migrationBuilder.DropForeignKey(
                name: "FK_Activities_Modules_ModuleId",
                table: "Activities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityTypes",
                table: "ActivityTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Activities",
                table: "Activities");

            migrationBuilder.RenameTable(
                name: "ActivityTypes",
                newName: "ModuleActivityTypes");

            migrationBuilder.RenameTable(
                name: "Activities",
                newName: "ModuleActivities");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ModuleId",
                table: "ModuleActivities",
                newName: "IX_ModuleActivities_ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Activities_ActivityTypeId",
                table: "ModuleActivities",
                newName: "IX_ModuleActivities_ActivityTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleActivityTypes",
                table: "ModuleActivityTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModuleActivities",
                table: "ModuleActivities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivities_ModuleActivityTypes_ActivityTypeId",
                table: "ModuleActivities",
                column: "ActivityTypeId",
                principalTable: "ModuleActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivities_Modules_ModuleId",
                table: "ModuleActivities",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
