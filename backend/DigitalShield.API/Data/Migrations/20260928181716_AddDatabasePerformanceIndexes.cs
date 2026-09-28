using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShield.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabasePerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Scenarios_FraudCategoryId",
                table: "Scenarios");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_FraudCategoryId",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_LearningModules_FraudCategoryId",
                table: "LearningModules");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgress_UserId_StartedAt",
                table: "UserProgress",
                columns: new[] { "UserId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_FraudCategoryId_Title",
                table: "Scenarios",
                columns: new[] { "FraudCategoryId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_FraudCategoryId_Title",
                table: "Quizzes",
                columns: new[] { "FraudCategoryId", "Title" });

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId_StartedAt",
                table: "QuizAttempts",
                columns: new[] { "UserId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_LearningModules_FraudCategoryId_Order",
                table: "LearningModules",
                columns: new[] { "FraudCategoryId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserProgress_UserId_StartedAt",
                table: "UserProgress");

            migrationBuilder.DropIndex(
                name: "IX_Scenarios_FraudCategoryId_Title",
                table: "Scenarios");

            migrationBuilder.DropIndex(
                name: "IX_Quizzes_FraudCategoryId_Title",
                table: "Quizzes");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_UserId_StartedAt",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_LearningModules_FraudCategoryId_Order",
                table: "LearningModules");

            migrationBuilder.CreateIndex(
                name: "IX_Scenarios_FraudCategoryId",
                table: "Scenarios",
                column: "FraudCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_FraudCategoryId",
                table: "Quizzes",
                column: "FraudCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningModules_FraudCategoryId",
                table: "LearningModules",
                column: "FraudCategoryId");
        }
    }
}
