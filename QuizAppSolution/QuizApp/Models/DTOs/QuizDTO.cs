namespace QuizApp.Models.DTOs
{
    public class QuizDTO
    {
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public int QuizCreatedBy { get; set; }
        public List<int> QuestionIds { get; set; } = new List<int>();
    }
}
