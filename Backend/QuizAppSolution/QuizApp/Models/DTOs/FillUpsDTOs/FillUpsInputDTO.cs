using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Models.DTOs.FillUpsDTOs
{
    [ExcludeFromCodeCoverage]
    public class FillUpsInputDTO
    {
        [Required(ErrorMessage = "Question text is required.")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "Points value is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Points must be a non-negative number.")]
        public decimal Points { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Difficulty level is required.")]
        public DifficultyLevel DifficultyLevel { get; set; }

        [Required(ErrorMessage = "Correct answer is required.")]
        public string? CorrectAnswer { get; set; }
    }
}
