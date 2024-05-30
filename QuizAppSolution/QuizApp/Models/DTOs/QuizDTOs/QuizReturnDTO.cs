namespace QuizApp.Models.DTOs.QuizDTOs
{
    public class QuizReturnDTO
    {
        public int QuizId { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int NumOfQuestions { get; set; }
        public int QuizCreatedBy { get; set; }
        public bool IsMultpleAllowed { get; set; }
        public decimal TotalPoints { get; set; }
        public TimeSpan Timelimit { get; set; }
        public List<int> QuizQuestions { get; set; }
    }
}
