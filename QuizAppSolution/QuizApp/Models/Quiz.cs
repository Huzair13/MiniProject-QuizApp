using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class Quiz
    {
        [Key]
        public int Id { get; set; }
        public string QuizName { get; set; }
        public string QuizDescription { get; set; }
        public string QuizType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int NumOfQuestions { get; set; }
        public decimal TotalPoints { get; set; }

        public int QuizCreatedBy { get; set; }
        public Teacher QuizCreatedByUser { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; }

    }
}
