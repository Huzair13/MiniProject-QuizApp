using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Migrations
{
    public partial class quizTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "TimeLimit",
                table: "Quizzes",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 202,
                column: "CreatedDate",
                value: new DateTime(2024, 5, 29, 23, 3, 54, 450, DateTimeKind.Local).AddTicks(9225));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 201,
                column: "CreatedDate",
                value: new DateTime(2024, 5, 29, 23, 3, 54, 450, DateTimeKind.Local).AddTicks(9198));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "TimeLimit" },
                values: new object[] { new DateTime(2024, 5, 29, 23, 3, 54, 450, DateTimeKind.Local).AddTicks(9286), new TimeSpan(0, 0, 30, 0, 0) });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeLimit",
                table: "Quizzes");

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 202,
                column: "CreatedDate",
                value: new DateTime(2024, 5, 28, 9, 56, 23, 836, DateTimeKind.Local).AddTicks(2085));

            migrationBuilder.UpdateData(
                table: "Questions",
                keyColumn: "Id",
                keyValue: 201,
                column: "CreatedDate",
                value: new DateTime(2024, 5, 28, 9, 56, 23, 836, DateTimeKind.Local).AddTicks(2059));

            migrationBuilder.UpdateData(
                table: "Quizzes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2024, 5, 28, 9, 56, 23, 836, DateTimeKind.Local).AddTicks(2096));
        }
    }
}
