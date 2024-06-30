using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApp.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EducationQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoinsEarned = table.Column<int>(type: "int", nullable: true),
                    NumsOfQuizAttended = table.Column<int>(type: "int", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumsOfQuestionsCreated = table.Column<int>(type: "int", nullable: true),
                    NumsOfQuizCreated = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Points = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuestionCreatedBy = table.Column<int>(type: "int", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Choice1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Choice2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Choice3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Choice4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorrectChoice = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Users_QuestionCreatedBy",
                        column: x => x.QuestionCreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumOfQuestions = table.Column<int>(type: "int", nullable: false),
                    TotalPoints = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuizCreatedBy = table.Column<int>(type: "int", nullable: false),
                    isMultipleAttemptAllowed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quizzes_Users_QuizCreatedBy",
                        column: x => x.QuizCreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsersDetails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordHashKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UsersDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuizQuestions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Responses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScoredPoints = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuizId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Responses_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Responses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponseId = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SubmittedAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResponseAnswers_Responses_ResponseId",
                        column: x => x.ResponseId,
                        principalTable: "Responses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "DateOfBirth", "Designation", "Email", "MobileNumber", "Name", "NumsOfQuestionsCreated", "NumsOfQuizCreated", "UserType" },
                values: new object[] { 101, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "HOD", "janu@gmail.com", "1234567890", "Janu", 2, 1, "Teacher" });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Category", "CorrectAnswer", "CreatedDate", "DifficultyLevel", "Points", "QuestionCreatedBy", "QuestionText", "QuestionType" },
                values: new object[] { 202, "Science", "Jupiter", new DateTime(2024, 5, 27, 13, 3, 23, 67, DateTimeKind.Local).AddTicks(2617), 1, 15m, 101, "What is the largest planet in our solar system?", "Fillups" });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "Category", "Choice1", "Choice2", "Choice3", "Choice4", "CorrectChoice", "CreatedDate", "DifficultyLevel", "Points", "QuestionCreatedBy", "QuestionText", "QuestionType" },
                values: new object[] { 201, "Geography", "Paris", "London", "Berlin", "Rome", "Paris", new DateTime(2024, 5, 27, 13, 3, 23, 67, DateTimeKind.Local).AddTicks(2588), 0, 10m, 101, "What is the capital of France?", "MultipleChoice" });

            migrationBuilder.InsertData(
                table: "Quizzes",
                columns: new[] { "Id", "CreatedOn", "NumOfQuestions", "QuizCreatedBy", "QuizDescription", "QuizName", "QuizType", "TotalPoints", "isMultipleAttemptAllowed" },
                values: new object[] { 1, new DateTime(2024, 5, 27, 13, 3, 23, 67, DateTimeKind.Local).AddTicks(2631), 2, 101, "A sample quiz", "Sample Quiz", "Practice", 25m, false });

            migrationBuilder.InsertData(
                table: "QuizQuestions",
                columns: new[] { "Id", "QuestionId", "QuizId" },
                values: new object[] { 301, 201, 1 });

            migrationBuilder.InsertData(
                table: "QuizQuestions",
                columns: new[] { "Id", "QuestionId", "QuizId" },
                values: new object[] { 302, 202, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionCreatedBy",
                table: "Questions",
                column: "QuestionCreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuestionId",
                table: "QuizQuestions",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestions_QuizId",
                table: "QuizQuestions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_QuizCreatedBy",
                table: "Quizzes",
                column: "QuizCreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseAnswers_QuestionId",
                table: "ResponseAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponseAnswers_ResponseId",
                table: "ResponseAnswers",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_QuizId",
                table: "Responses",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Responses_UserId",
                table: "Responses",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuizQuestions");

            migrationBuilder.DropTable(
                name: "ResponseAnswers");

            migrationBuilder.DropTable(
                name: "UsersDetails");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Responses");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
