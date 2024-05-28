namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizInputDTO
    {
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public bool IsMultipleAttemptAllowed { get; set; }
        public List<int> QuestionIds { get; set; }
    }
}
