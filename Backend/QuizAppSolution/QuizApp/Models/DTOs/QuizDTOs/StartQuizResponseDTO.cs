namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class StartQuizResponseDTO
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public List<QuizResponseQuestionDTO> Questions { get; set; }
    }
}
