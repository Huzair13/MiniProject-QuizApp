using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public enum DifficultyLevel
    {
        Easy,
        Medium,
        Hard
    }

    public class Question
    {
        [Key]
        public int Id { get; set; }


        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public decimal Points { get; set; }
        public string Category { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public DateTime CreatedDate { get; set; }

        public int QuestionCreatedBy { get; set; }
        public Teacher CreatedByUser { get; set; }


        public ICollection<QuizQuestion> QuizQuestions { get; set; }


    }
}
