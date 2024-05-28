namespace QuizApp.Models.DTOs.ResponseDTO
{
    public class SubmitAnswerDTO
    {
        public int? UserId { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
