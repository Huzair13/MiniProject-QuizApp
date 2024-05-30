namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizDTO
    {
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public bool IsMultpleAttemptAllowed { get; set; }
        public int QuizCreatedBy { get; set; }
        public List<int> QuestionIds { get; set; } = new List<int>();
        public TimeSpan TimeLimit { get; set; }
    }
}
