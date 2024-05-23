namespace QuizApp.Models
{
    public class Student :User
    {
        public string EducationQualification {  get; set; }
        public int? CoinsEarned { get; set; }
        public int? NumsOfQuizAttended { get; set; }
    }
}
