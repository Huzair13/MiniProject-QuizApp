using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs
{
    public class UpdateQuestionDTO
    {
        [Required(ErrorMessage = "Question ID is required.")]
        public int Id { get; set; }

        [StringLength(500, ErrorMessage = "Question text cannot be longer than 500 characters.")]
        public string? QuestionText { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Points must be a non-negative number.")]
        public decimal? Points { get; set; }

        [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters.")]
        public string? Category { get; set; }

        public DifficultyLevel? DifficultyLevel { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice1 { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice2 { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice3 { get; set; }

        [StringLength(100, ErrorMessage = "Choice cannot be longer than 100 characters.")]
        public string? Choice4 { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
