using System.Diagnostics.CodeAnalysis;

namespace QuizApp.Models.DTOs.UserDTOs
{
    [ExcludeFromCodeCoverage]
    public class UserDTO : User
    {
        public string Password { get; set; }
    }
}
