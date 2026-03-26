using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameModuleActivityToActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivities_ModuleActivityTypes_ModuleActivityTypeId",
                table: "ModuleActivities");

            migrationBuilder.RenameColumn(
                name: "ModuleActivityTypeId",
                table: "ModuleActivities",
                newName: "ActivityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleActivities_ModuleActivityTypeId",
                table: "ModuleActivities",
                newName: "IX_ModuleActivities_ActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivities_ModuleActivityTypes_ActivityTypeId",
                table: "ModuleActivities",
                column: "ActivityTypeId",
                principalTable: "ModuleActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModuleActivities_ModuleActivityTypes_ActivityTypeId",
                table: "ModuleActivities");

            migrationBuilder.RenameColumn(
                name: "ActivityTypeId",
                table: "ModuleActivities",
                newName: "ModuleActivityTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_ModuleActivities_ActivityTypeId",
                table: "ModuleActivities",
                newName: "IX_ModuleActivities_ModuleActivityTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ModuleActivities_ModuleActivityTypes_ModuleActivityTypeId",
                table: "ModuleActivities",
                column: "ModuleActivityTypeId",
                principalTable: "ModuleActivityTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
