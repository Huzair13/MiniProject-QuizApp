namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizUpdateDTO
    {
        public int QuizID { get; set; }
        public string? QuizName { get; set; }
        public string? QuizDescription { get; set; }
        public string? QuizType { get; set; }
        public bool? IsMultpleAttemptAllowed { get; set; }
        public List<int>? QuestionIds { get; set; } = new List<int>();
    }
}
