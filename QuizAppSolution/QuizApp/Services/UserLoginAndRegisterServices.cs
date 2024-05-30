
using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs.UserDTOs;
using QuizApp.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace QuizApp.Services
{
    public class UserLoginAndRegisterServices : IUserLoginAndRegisterServices
    {
        //REPOSITORY INITIALIZATION
        private readonly IRepository<int, UserDetails> _userDetailsRepo;
        private readonly ITokenServices _tokenServices;
        private readonly IRepository<int,Teacher> _teacherRepo;
        private readonly IRepository<int, Student> _studentRepo;
        private readonly IRepository<int, User> _userRepo;
        private readonly ILogger<UserLoginAndRegisterServices> _logger;

        //DEPENDENCY INJECTION
        public UserLoginAndRegisterServices(IRepository<int, User> userRepo,
                            IRepository<int, UserDetails> userDetailsRepo,
                            ITokenServices tokenServices, IRepository<int, Teacher> teacherRepo,
                            IRepository<int, Student> studentRepo, ILogger<UserLoginAndRegisterServices> logger
                            )
        {
            _userDetailsRepo = userDetailsRepo;
            _tokenServices = tokenServices;
            _teacherRepo = teacherRepo;
            _studentRepo = studentRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        //LOGIN SERVICE
        public async Task<LoginReturnDTO> Login(UserLoginDTO loginDTO)
        {
            try
            {
                var userDB = await _userDetailsRepo.Get(loginDTO.UserId);
                string userRole = await CheckUserRole(loginDTO);
                if (userDB == null)
                {
                    throw new UnauthorizedUserException("Invalid username or password");
                }
                HMACSHA512 hMACSHA = new HMACSHA512(userDB.PasswordHashKey);
                var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));
                bool isPasswordSame = ComparePassword(encrypterPass, userDB.Password);
                if (isPasswordSame)
                {
                    return await LoginBasedOnUserRole(loginDTO,userRole);    
                }
                throw new UnauthorizedUserException("Invalid username or password");
            }
            catch (NoSuchUserException ex)
            {
                _logger.LogError(ex, "User Not Found at Login service");
                throw new UnauthorizedUserException("Invalid username or password");
            }
            catch(UnauthorizedUserException ex)
            {
                _logger.LogError(ex, "Unauthorized at Login service");
                throw ex;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An Error occured at Login service");
                throw new Exception(ex.Message);
            }
        }

        //LOGIN BASED ON ROLES (TEACHER OR STUDENT)
        private async Task<LoginReturnDTO> LoginBasedOnUserRole(UserLoginDTO loginDTO, string userRole)
        {
            if (userRole == "Teacher")
            {
                var user = await _teacherRepo.Get(loginDTO.UserId);
                LoginReturnDTO loginReturnDTO = await MapUsereToLoginReturn(user);
                return loginReturnDTO;
            }
            else if (userRole == "Student")
            {
                var user = await _studentRepo.Get(loginDTO.UserId);
                LoginReturnDTO loginReturnDTO = await MapUsereToLoginReturn(user);
                return loginReturnDTO;
            }
            return null;
        }

        //CHECK USER ROLE
        private async Task<string> CheckUserRole(UserLoginDTO loginDTO)
        {
            User user = await _userRepo.Get(loginDTO.UserId);
            return user.UserType;
        }

        //MAP USER TO LOGIN RETURN
        private async Task<LoginReturnDTO> MapUsereToLoginReturn(User user)
        {
            string role = user.UserType;
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.userID = user.Id;
            returnDTO.Role = role;
            returnDTO.Token = await _tokenServices.GenerateToken(user);
            return returnDTO;
        }


        //COMPARE PASSWORD
        private bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }

        //REGISTER SERVICE
        public async Task<RegisterReturnDTO> Register(UserRegisterInputDTO userInputDTO)
        {
            if (userInputDTO.UserType == "Teacher")
            {
                return await TeacherRegister(userInputDTO);
            }
            else if(userInputDTO.UserType == "Student")
            {
                return await StudentRegister(userInputDTO);
            }
            return null;
        }

        //STUDENT REGISTER
        private async Task<RegisterReturnDTO> StudentRegister(UserRegisterInputDTO userInputDTO)
        {
            Student student = null;
            UserDetails userDetail = null;
            try
            {
                student = await MapUserDTOToStudent(userInputDTO);
                userDetail = await MapUserDTOToUserDetails(userInputDTO);
                var existingUsers = await _studentRepo.Get();
                var result = existingUsers.FirstOrDefault(u => u.Email == userInputDTO.Email);
                var resultPhone = existingUsers.FirstOrDefault(u => u.MobileNumber == userInputDTO.MobileNumber);
                if (result != null || resultPhone != null)
                {
                    throw new UserAlreadyExistsException();
                }
                student = await _studentRepo.Add(student);
                userDetail.UserId = student.Id;
                userDetail = await _userDetailsRepo.Add(userDetail);
                RegisterReturnDTO registerReturnDTO = await MapStudentToRegisterReturnDTO(student);
                return registerReturnDTO;
            }
            catch (UserAlreadyExistsException ex)
            {
                _logger.LogError(ex, "User Already Exists Error at Student Register service");
                throw new UserAlreadyExistsException(ex.Message);
            }
            catch (Exception) 
            { 

            }
            await RevertStudentAction(student, userDetail);
            throw new UnableToRegisterException("Not able to register at this moment");
        }

        //REVERT STUDENT ACTION IF REGISTER FAILS
        private async Task RevertStudentAction(Student? student, UserDetails? userDetail)
        {
            if (student != null)
                await RevertStudentInsert(student);
            if (userDetail != null && student == null)
                await RevertUserDetailsInsert(userDetail);
        }
        
        //TEACHER REGISTER
        private async Task<RegisterReturnDTO> TeacherRegister(UserRegisterInputDTO userInputDTO)
        {
            Teacher teacher = null;
            UserDetails userDetail = null;
            try
            {
                teacher = await MapUserDTOToTeacher(userInputDTO);
                userDetail = await MapUserDTOToUserDetails(userInputDTO);

                var existingUsers = await _teacherRepo.Get();

                var result = existingUsers.FirstOrDefault(u => u.Email == teacher.Email);
                var phoneResult = existingUsers.FirstOrDefault(u => u.MobileNumber == teacher.MobileNumber);
                if (result != null || phoneResult != null)
                {
                    throw new UserAlreadyExistsException();
                }
                var teacherResult = await _teacherRepo.Add(teacher);
                userDetail.UserId = teacherResult.Id;
                userDetail = await _userDetailsRepo.Add(userDetail);
                RegisterReturnDTO registerReturnDTO = await MapTeacherToRegisterReturnDTO(teacher);
                return registerReturnDTO;
            }
            catch (UserAlreadyExistsException ex)
            {
                _logger.LogError(ex, "User Already Exists Error at Teacher Register service");
                throw new UserAlreadyExistsException();
            }
            catch (Exception) { }
            await RevertTeacherAction(teacher, userDetail);
            throw new UnableToRegisterException("Not able to register at this moment");
        }

        //REVERT ACTION IF TEACHER ACTION FAILS
        private async Task RevertTeacherAction(Teacher teacher, UserDetails userDetail)
        {
            if (teacher != null)
                await ReverTeacherInsert(teacher);
            if (userDetail != null && teacher == null)
                await RevertUserDetailsInsert(userDetail);
        }

        private async Task RevertUserDetailsInsert(UserDetails userDetails)
        {
            await _userDetailsRepo.Delete(userDetails.UserId);
        }

        private async Task ReverTeacherInsert(Teacher teacher) 
        {
            await _teacherRepo.Delete(teacher.Id);
        }

        private async Task RevertStudentInsert(Student student)
        {
            await _studentRepo.Delete(student.Id);
        }

        //MAP TEACHER TO REGISTER RETURN DTO
        public async Task<RegisterReturnDTO> MapTeacherToRegisterReturnDTO(Teacher teacher)
        {
            RegisterReturnDTO TeacherReturn = new RegisterReturnDTO();
            TeacherReturn.Id = teacher.Id;
            TeacherReturn.Name = teacher.Name;
            TeacherReturn.MobileNumber= teacher.MobileNumber;
            TeacherReturn.Email = teacher.Email;
            TeacherReturn.DateOfBirth= teacher.DateOfBirth;
            TeacherReturn.Role = "Teacher";
            return TeacherReturn;
        }

        //MAP STUDENT TO REGISTER RETURN DTO
        public async Task<RegisterReturnDTO> MapStudentToRegisterReturnDTO(Student student)
        {
            RegisterReturnDTO studentReturn = new RegisterReturnDTO();
            studentReturn.Id = student.Id;
            studentReturn.Name = student.Name;
            studentReturn.MobileNumber = student.MobileNumber;
            studentReturn.Email = student.Email;
            studentReturn.DateOfBirth = student.DateOfBirth;
            studentReturn.Role = "Student";
            return studentReturn;
        }

        //MAP USERDTO TO TEACHER
        public async Task<Teacher> MapUserDTOToTeacher(UserRegisterInputDTO userDTO)
        {
            Teacher teacher = new Teacher();
            teacher.MobileNumber = userDTO.MobileNumber;
            teacher.Name = userDTO.Name;
            teacher.Email = userDTO.Email;
            teacher.DateOfBirth = userDTO.DateOfBirth;
            teacher.Designation = userDTO.Designation;
            return teacher;
             
        }

        //MAP USERDTO TO STUDENT
        public async Task<Student> MapUserDTOToStudent(UserRegisterInputDTO userDTO)
        {
            Student student = new Student();
            student.MobileNumber = userDTO.MobileNumber;
            student.Name = userDTO.Name;
            student.Email = userDTO.Email;
            student.DateOfBirth = userDTO.DateOfBirth;
            student.EducationQualification= userDTO.EducationQualification;
            return student;

        }

        //MAP SERDTO TO USER DETAILS
        private async Task<UserDetails> MapUserDTOToUserDetails(UserRegisterInputDTO userDTO)
        {
            UserDetails userDetails = new UserDetails();
            HMACSHA512 hMACSHA = new HMACSHA512();

            userDetails.PasswordHashKey = hMACSHA.Key;
            userDetails.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userDTO.Password));

            return userDetails;
        }

    }
}
