using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Examination_System.Migrations
{
    /// <inheritdoc />
    public partial class initCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Courses_CourseId1",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Instructors_InstructorId1",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CourseId1",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_InstructorId1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "InstructorId1",
                table: "Questions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseId1",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId1",
                table: "Questions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questions_CourseId1",
                table: "Questions",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_InstructorId1",
                table: "Questions",
                column: "InstructorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Courses_CourseId1",
                table: "Questions",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Instructors_InstructorId1",
                table: "Questions",
                column: "InstructorId1",
                principalTable: "Instructors",
                principalColumn: "Id");
        }
    }
}
