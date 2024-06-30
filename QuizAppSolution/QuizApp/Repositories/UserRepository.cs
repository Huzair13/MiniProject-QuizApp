using QuizApp.Contexts;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Repositories
{
    public class UserRepository : GenericUserRepository<User>, IRepository<int, User>
    {
        public UserRepository(QuizAppContext context) : base(context)
        {
        }

    }
}
