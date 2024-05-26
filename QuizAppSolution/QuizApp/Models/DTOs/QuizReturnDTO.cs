namespace QuizApp.Models.DTOs
{
    public class QuizReturnDTO
    {
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int NumOfQuestions { get; set; }
        public int QuizCreatedBy { get; set; }
        public decimal TotalPoints { get; set; }
        public List<int> QuizQuestions { get; set; }
    }
}
