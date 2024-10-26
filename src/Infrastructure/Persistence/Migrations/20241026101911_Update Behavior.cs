using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_Assignment_AssignmentId",
                table: "user_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_Status_StatusId",
                table: "user_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_Group_GroupId",
                table: "user_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_User_UserId",
                table: "user_groups");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "assignments",
                column: "course_id",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_Assignment_AssignmentId",
                table: "user_assignments",
                column: "assignment_id",
                principalTable: "assignments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_Status_StatusId",
                table: "user_assignments",
                column: "status_id",
                principalTable: "statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_Group_GroupId",
                table: "user_groups",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_User_UserId",
                table: "user_groups",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_Assignment_AssignmentId",
                table: "user_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAssignment_Status_StatusId",
                table: "user_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_Group_GroupId",
                table: "user_groups");

            migrationBuilder.DropForeignKey(
                name: "FK_UserGroup_User_UserId",
                table: "user_groups");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Course_CourseId",
                table: "assignments",
                column: "course_id",
                principalTable: "courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_Assignment_AssignmentId",
                table: "user_assignments",
                column: "assignment_id",
                principalTable: "assignments",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAssignment_Status_StatusId",
                table: "user_assignments",
                column: "status_id",
                principalTable: "statuses",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_Group_GroupId",
                table: "user_groups",
                column: "group_id",
                principalTable: "groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroup_User_UserId",
                table: "user_groups",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
