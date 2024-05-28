namespace QuizApp.Models.DTOs.ResponseDTO
{
    public class SubmitAllAnswersDTO
    {
        public int? UserId { get; set; }
        public int QuizId { get; set; }
        public Dictionary<int, string> QuestionAnswers { get; set; }
    }
}
