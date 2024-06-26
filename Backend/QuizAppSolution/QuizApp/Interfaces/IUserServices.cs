using QuizApp.Models;
using QuizApp.Models.DTOs.UserDTOs;

namespace QuizApp.Interfaces
{
    public interface IUserServices
    {
        public Task<User> GetUserById(int UserId);
        public Task<StudentReturnDTO> ViewStudentProfile(int userId);
        public Task<TeacherReturnDTO> ViewTeacherProfile(int userId);
    }
}
