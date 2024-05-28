namespace QuizApp.Models.DTOs.FillUpsDTOs
{
    public class FillUpsDTO
    {
        public string QuestionText { get; set; }
        public decimal Points { get; set; }
        public string Category { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string? CorrectAnswer { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string QuestionType { get; set; }
    }
}
