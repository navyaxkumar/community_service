using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalShield.API.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefineDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_UserProgress_ProgressPercentage_Range",
                table: "UserProgress",
                sql: "[ProgressPercentage] >= 0 AND [ProgressPercentage] <= 100");

            migrationBuilder.AddCheckConstraint(
                name: "CK_QuizQuestions_Order_NonNegative",
                table: "QuizQuestions",
                sql: "[Order] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_QuizOptions_Order_NonNegative",
                table: "QuizOptions",
                sql: "[Order] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_QuizAttempts_Score_NonNegative",
                table: "QuizAttempts",
                sql: "[Score] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_QuizAttempts_Score_NotGreaterThanTotalQuestions",
                table: "QuizAttempts",
                sql: "[Score] <= [TotalQuestions]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_QuizAttempts_TotalQuestions_NonNegative",
                table: "QuizAttempts",
                sql: "[TotalQuestions] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_LearningModules_Order_NonNegative",
                table: "LearningModules",
                sql: "[Order] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Badges_RequiredPoints_NonNegative",
                table: "Badges",
                sql: "[RequiredPoints] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_UserProgress_ProgressPercentage_Range",
                table: "UserProgress");

            migrationBuilder.DropCheckConstraint(
                name: "CK_QuizQuestions_Order_NonNegative",
                table: "QuizQuestions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_QuizOptions_Order_NonNegative",
                table: "QuizOptions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_QuizAttempts_Score_NonNegative",
                table: "QuizAttempts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_QuizAttempts_Score_NotGreaterThanTotalQuestions",
                table: "QuizAttempts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_QuizAttempts_TotalQuestions_NonNegative",
                table: "QuizAttempts");

            migrationBuilder.DropCheckConstraint(
                name: "CK_LearningModules_Order_NonNegative",
                table: "LearningModules");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Badges_RequiredPoints_NonNegative",
                table: "Badges");
        }
    }
}
