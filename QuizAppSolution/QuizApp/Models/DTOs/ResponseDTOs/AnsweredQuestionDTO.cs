namespace QuizApp.Models.DTOs.ResponseDTOs
{
    public class AnsweredQuestionDTO
    {
        public int QuestionId { get; set; }
        public string SubmittedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
    }
}
