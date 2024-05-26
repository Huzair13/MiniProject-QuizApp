namespace QuizApp.Models.DTOs
{
    public class MCQDTO
    {
        public string QuestionText { get; set; }
        public decimal Points { get; set; }
        public string Category { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public string? Choice1 { get; set; }
        public string? Choice2 { get; set; }
        public string? Choice3 { get; set; }
        public string? Choice4 { get; set; }
        public string? CorrectAnswer { get; set; }
        public DateTime CreatedDate { get; set;}
        public int CreatedBy { get; set; }
        public string QuestionType { get; set; }
    }
}
