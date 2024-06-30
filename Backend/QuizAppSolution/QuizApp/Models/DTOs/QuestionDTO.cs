namespace QuizApp.Models.DTOs
{
    public class QuestionDTO
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public decimal Points { get; set; }
        public string Category { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public DateTime CreatedDate { get; set; }
        public int QuestionCreatedBy { get; set; }
        public string QuestionType { get; set; }
    }
}
