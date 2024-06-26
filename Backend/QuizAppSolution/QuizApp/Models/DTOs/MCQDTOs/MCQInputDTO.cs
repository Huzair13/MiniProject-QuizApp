using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Models.DTOs.MCQDTOs
{
    [ExcludeFromCodeCoverage]
    public class MCQInputDTO
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

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice1 { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice2 { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice3 { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice4 { get; set; }

        [Required(ErrorMessage = "Correct answer is required.")]
        public string? CorrectAnswer { get; set; }
    }
}
