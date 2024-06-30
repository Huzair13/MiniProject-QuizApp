namespace QuizApp.Models.DTOs.UserDTOs
{
    public class StudentReturnDTO
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

        public string EducationQualification { get; set; }
        public int? CoinsEarned { get; set; }
        public int? NumsOfQuizAttended { get; set; }

    }
}
