using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs.UserDTOs;
using QuizApp.Repositories;
using QuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppTest.ServicesTest
{
    public class UserRepositoryTesting
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IRepository<int, Teacher> teacherRepo;
        IRepository<int, Student> studentRepo;
        Mock<ILogger<UserLoginAndRegisterServices>> mockLogger;

        [SetUp]
        public async Task setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                    .UseInMemoryDatabase("QuizAppDummyDB");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);


            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);
            teacherRepo = new TeacherRepository(quizAppContext);
            studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, User> userRepo = new UserRepository(quizAppContext);

            mockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();

            userLoginAndRegisterServices = new UserLoginAndRegisterServices
                                                    (userRepo, userDetailsRepo, tokenServices, 
                                                    teacherRepo, studentRepo,mockLogger.Object);



        }
        [Test]
        public async Task TeacherRegisterTest()
        {
            //Arrange
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Vishal",
                Email = "vishal@gmail.com",
                MobileNumber = "9888777766",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "vishal123",
                Designation = "UG-Teacher",
                UserType = "Teacher"
            };

            //Action
            var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);

            //Assert
            Assert.NotNull(result.Id);
        }

        [Test]
        public async Task TeacherLoginTest()
        {
            //Arrange
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Saren",
                Email = "saren@gmail.com",
                MobileNumber = "98776766545",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "saren123",
                Designation = "HOD",
                UserType = "Teacher"
            };
            var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);

            int userId = result.Id;

            UserLoginDTO userLoginDTO = new UserLoginDTO
            {
                UserId = userId,
                Password = "saren123"
            };

            //Action
            var logInResult = await userLoginAndRegisterServices.Login(userLoginDTO);

            //Assert
            Assert.NotNull(logInResult.Token);
        }

        [Test]
        public async Task StudentRegisterTest()
        {
            //Arrange
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Shree",
                Email = "shree@gmail.com",
                MobileNumber = "9899998877",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "shre123",
                EducationQualification = "B.Tech - AI and DS",
                UserType = "Student"
            };

            //Action
            var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);

            //Assert
            Assert.NotNull(result.Id);
        }

        [Test]
        public async Task StudentLoginTest()
        {
            //Arrange
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Praveen",
                Email = "praveen@gmail.com",
                MobileNumber = "9877665544",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "praveen123",
                EducationQualification = "B.Tech - AI and DS",
                UserType = "Student"
            };
            var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);

            int userId = result.Id;

            UserLoginDTO userLoginDTO = new UserLoginDTO
            {
                UserId = userId,
                Password = "praveen123"
            };

            //Action
            var logInResult = await userLoginAndRegisterServices.Login(userLoginDTO);

            //Assert
            Assert.NotNull(logInResult.Token);
        }

        [Test]
        public async Task LoginExceptionTest()
        {
            // Arrange
            UserLoginDTO userLoginDTO = new UserLoginDTO
            {
                UserId = 299,
                Password = "saren123"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () =>
                await userLoginAndRegisterServices.Login(userLoginDTO)
            );

            // Assert
            Assert.AreEqual($"Invalid username or password", exception.Message);
        }

        [Test]
        public async Task LoginFailureTest()
        {
            //Arrange
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Janu",
                Email = "janu@gmail.com",
                MobileNumber = "7687988998",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "janu123",
                EducationQualification = "B.Tech -AI and ML",
                UserType = "Student"
            };

            var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);

            UserLoginDTO userLoginDTO = new UserLoginDTO
            {
                UserId = result.Id,
                Password = "saren123"
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedUserException>(async () =>
                await userLoginAndRegisterServices.Login(userLoginDTO)
            );

            // Assert
            Assert.AreEqual($"Invalid username or password", exception.Message);
        }


        [Test]
        public async Task TeacherRegisterExceptionTest()
        {
            Teacher teacher = new Teacher
            {
                Name = "Sindhu",
                Email = "sindhu@gmail.com",
                MobileNumber = "9877665566",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Designation = "HOD"
            };
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Sindhu",
                Email = "sindhu@gmail.com",
                MobileNumber = "9877665566",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "sindhu123",
                Designation = "HOD",
                UserType = "Teacher"
            };

            await teacherRepo.Add(teacher);

            //Action
            var exception = Assert.ThrowsAsync<UserAlreadyExistsException>(() => userLoginAndRegisterServices
                                            .Register(userRegisterInputDTO));

            //Assert
            Assert.AreEqual("User with the same EmailID or Mobile Number is already Registered. Please try to login", exception.Message);

        }

        [Test]
        public async Task StudentRegisterExceptionTest()
        {
            Student student = new Student
            {
                Name = "Sindhu",
                Email = "sindhu@gmail.com",
                MobileNumber = "9877665566",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                EducationQualification = "B.tech - AI and DS"
            };
            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Sindhu",
                Email = "sindhu@gmail.com",
                MobileNumber = "9877665566",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "sindhu123",
                Designation = "HOD",
                UserType = "Student"
            };

            await studentRepo.Add(student);

            //Action
            var exception = Assert.ThrowsAsync<UserAlreadyExistsException>(() => userLoginAndRegisterServices
                                            .Register(userRegisterInputDTO));

            //Assert
            Assert.AreEqual("User with the same EmailID or Mobile Number is already Registered. Please try to login", exception.Message);

        }

    }
}
