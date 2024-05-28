namespace QuizApp.Models.DTOs.UserDTOs
{
    public class LoginReturnDTO
    {
        public int userID { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
