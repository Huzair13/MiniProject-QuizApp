using QuizApp.Models;
using QuizApp.Models.DTOs.UserDTOs;

namespace QuizApp.Interfaces
{
    public interface IUserLoginAndRegisterServices
    {
        public Task<LoginReturnDTO> Login(UserLoginDTO loginDTO);
        public Task<RegisterReturnDTO> Register(UserRegisterInputDTO userInputDTO);
    }
}
