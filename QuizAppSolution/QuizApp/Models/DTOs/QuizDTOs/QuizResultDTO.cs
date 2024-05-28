using QuizApp.Models.DTOs.ResponseDTOs;

namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizResultDTO
    {
        public int ResponseId { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public decimal Score { get; set; }
        public List<AnsweredQuestionDTO> AnsweredQuestions { get; set; }
    }
}
