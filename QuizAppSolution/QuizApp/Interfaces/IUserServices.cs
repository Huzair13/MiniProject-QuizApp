using QuizApp.Models;

namespace QuizApp.Interfaces
{
    public interface IUserServices
    {
        public Task<User> GetUserById(int UserId);
    }
}
