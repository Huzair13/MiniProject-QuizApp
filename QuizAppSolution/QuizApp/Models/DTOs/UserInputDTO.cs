namespace QuizApp.Models.DTOs
{
    public class UserInputDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; }

        public string? EducationQualification { get; set; }
        public string? Designation { get; set; }
        public string userType { get; set; }

    }
}
