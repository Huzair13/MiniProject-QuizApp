using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs.UserDTOs;

namespace QuizApp.Services
{
    public class UserServices : IUserServices
    {
        //REPOSITORY INITIALIZATION
        private readonly IRepository<int, User> _userRepo;
        private readonly IRepository<int, Teacher> _teacherRepo;
        private readonly IRepository<int, Student> _studentRepo;
        private readonly ILogger<UserServices> _logger;

        //DEPENDENCY INJECTION
        public UserServices(IRepository<int, User> userRepo,IRepository<int,Teacher> teacherRepo,
                                IRepository<int,Student> studentRepo, ILogger<UserServices> logger)
        {
            _userRepo = userRepo;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _logger = logger;
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
                _logger.LogError(ex, "No User found");
                throw new NoSuchUserException(ex.Message);
            }
        }

        public async Task<TeacherReturnDTO> ViewTeacherProfile(int userId)
        {
            try
            {
                Teacher teacher = await _teacherRepo.Get(userId);
                var teacherReturn = await MapTeacherToTeacherReturnDTO(teacher);
                return teacherReturn;
            }
            catch(NoSuchUserException ex)
            {
                _logger.LogError(ex, "No User found ");
                throw ex;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "No User found");
                throw ex;
            }
        }

        public async Task<StudentReturnDTO> ViewStudentProfile(int userId)
        {
            try
            {
                Student student = await _studentRepo.Get(userId);
                StudentReturnDTO studentReturn = await MapStudentToStudentReturnDTO(student);
                return studentReturn;
            }
            catch (NoSuchUserException ex)
            {
                _logger.LogError(ex, "No User found");
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No User found");
                throw ex;
            }
        }

        private async Task<StudentReturnDTO> MapStudentToStudentReturnDTO(Student student)
        {
           StudentReturnDTO studentReturnDTO = new StudentReturnDTO() 
           { 
               Id = student.Id,
               Name = student.Name,
               DateOfBirth = student.DateOfBirth,
               CoinsEarned   = student.CoinsEarned,
               Email = student.Email,
               MobileNumber = student.MobileNumber,
               EducationQualification = student.EducationQualification,
               NumsOfQuizAttended = student.NumsOfQuizAttended
           };
            return studentReturnDTO;
        }

        private async Task<TeacherReturnDTO> MapTeacherToTeacherReturnDTO(Teacher teacher)
        {
            TeacherReturnDTO teacherReturnDTO = new TeacherReturnDTO()
            {
                Id = teacher.Id,
                Name = teacher.Name,
                Email = teacher.Email,
                DateOfBirth = teacher.DateOfBirth,
                Designation = teacher.Designation,
                MobileNumber = teacher.MobileNumber,
                NumsOfQuestionsCreated = teacher.NumsOfQuestionsCreated,
                NumsOfQuizCreated = teacher.NumsOfQuizCreated
            };
            return teacherReturnDTO;
        }
    }
}
