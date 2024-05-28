using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.DTOs.MCQDTOs
{
    public class MCQUpdateDTO
    {
        [Required (ErrorMessage = "Question ID can be empty")]
        public int QuestionId {  get; set; }
        public string? QuestionText { get; set; }
        public decimal? Points { get; set; }
        public string? Category { get; set; }
        public DifficultyLevel? DifficultyLevel { get; set; }
        public string? Choice1 { get; set; }
        public string? Choice2 { get; set; }
        public string? Choice3 { get; set; }
        public string? Choice4 { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
