namespace QuizApp.Models
{
    public class Teacher :User
    {
        public string Designation { get; set; }
        public int? NumsOfQuestionsCreated { get; set; }
        public int? NumsOfQuizCreated { get; set; }

        public ICollection<Question> QuestionsCreated { get; set; }
        public ICollection<Quiz> QuizCreated { get; set; }
    }
}
