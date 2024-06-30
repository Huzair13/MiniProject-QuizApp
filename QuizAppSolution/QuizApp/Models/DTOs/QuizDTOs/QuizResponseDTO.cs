using QuizApp.Models.DTOs.ResponseDTOs;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizResponseDTO
    {
        public int ResponseId { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public decimal Score { get; set; }
        public decimal totalPoints { get; set; }
        public decimal CorrectPercentage { get; set; }
        public List<AnsweredQuestionDTO> AnsweredQuestions { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
