using QuizApp.Models;

namespace QuizApp.Interfaces
{
    public interface ITokenServices
    {
        public Task<string> GenerateToken(User user);
    }
}
