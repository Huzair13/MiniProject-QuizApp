namespace QuizApp.Models.DTOs
{
    public class QuizInputDTO
    {
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public List<int> QuestionIds { get; set; }
    }
}
