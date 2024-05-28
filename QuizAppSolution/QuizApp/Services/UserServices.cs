using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;

namespace QuizApp.Services
{
    public class UserServices : IUserServices
    {
        //REPOSITORY INITIALIZATION
        private readonly IRepository<int, User> _userRepo;

        //DEPENDENCY INJECTION
        public UserServices(IRepository<int, User> userRepo)
        {
            _userRepo = userRepo;
        }

        //GET USER BY ID
        public async Task<User> GetUserById(int UserId)
        {
            try
            {
                var user = await _userRepo.Get(UserId);
                return user;
            }
            catch(NoSuchUserException ex)
            {
                throw new NoSuchUserException(ex.Message);
            }
        }
    }
}
