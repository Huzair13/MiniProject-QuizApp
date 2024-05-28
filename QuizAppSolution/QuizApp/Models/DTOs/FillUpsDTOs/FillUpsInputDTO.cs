namespace QuizApp.Models.DTOs.FillUpsDTOs
{
    public class FillUpsInputDTO
    {
        public string QuestionText { get; set; }
        public decimal Points { get; set; }
        public string Category { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string? CorrectAnswer { get; set; }
    }
}
