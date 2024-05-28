namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizResponseQuestionDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public List<string> Options { get; set; }
    }
}
