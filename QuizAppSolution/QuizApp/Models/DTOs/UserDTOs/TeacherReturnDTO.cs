namespace QuizApp.Models.DTOs.UserDTOs
{
    public class TeacherReturnDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age
        {
            get
            {
                DateTime today = DateTime.Today;
                int age = today.Year - DateOfBirth.Year;
                return age;
            }
        }

        public string Designation { get; set; }
        public int? NumsOfQuestionsCreated { get; set; }
        public int? NumsOfQuizCreated { get; set; }

    }
}
