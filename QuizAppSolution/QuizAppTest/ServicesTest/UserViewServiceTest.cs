using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Repositories;
using QuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppTest.ServicesTest
{
    public class UserViewServiceTest
    {
        QuizAppContext quizAppContext;
        IRepository<int, User> userRepo;
        IRepository<int, Student> studentRepo;
        IRepository<int, Teacher> teacherRepo;
        Mock<ILogger<UserServices>> mockLogger;
        IUserServices userServices;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                    .UseInMemoryDatabase("StudentRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);
            mockLogger = new Mock<ILogger<UserServices>>();

            userRepo = new UserRepository(quizAppContext);
            studentRepo = new StudentRepository(quizAppContext);
            teacherRepo = new TeacherRepository(quizAppContext);

            userServices = new UserServices(userRepo, teacherRepo, studentRepo, mockLogger.Object);
        }


        [Test]
        public async Task GetStudentSuccessTest()
        {
            //Arrange

            var result = await userRepo.Add(new Student
            {
                Name = "Vijay",
                DateOfBirth = new DateTime(2002 / 03 / 13),
                Email = "vijay@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.Tech - AI and DS"
            });

            //Action
            var Getresult = await userServices.GetUserById(result.Id);

            //Assert
            Assert.NotNull(Getresult);
            await userRepo.Delete(Getresult.Id);
        }

        [Test]
        public async Task GetStudentExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userServices.GetUserById(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetStudentProfileSuccessTest()
        {
            //Arrange
            var result = await userRepo.Add(new Student
            {
                Name = "Vijay",
                DateOfBirth = new DateTime(2002 / 03 / 13),
                Email = "vijay@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.Tech - AI and DS"
            });

            //Action
            var Getresult = await userServices.ViewStudentProfile(result.Id);

            //Assert
            Assert.NotNull(Getresult);
            await userRepo.Delete(Getresult.Id);
        }

        [Test]
        public async Task GetStudentProfileExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userServices.ViewStudentProfile(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetTeacherProfileSuccessTest()
        {
            //Arrange
            var result = await userRepo.Add(new Teacher
            {
                Name = "Vijay",
                DateOfBirth = new DateTime(2002 / 03 / 13),
                Email = "vijay@gmail.com",
                MobileNumber = "9677381857",
                Designation ="HOD"
            });

            //Action
            var Getresult = await userServices.ViewTeacherProfile(result.Id);

            //Assert
            Assert.NotNull(Getresult);
            await userRepo.Delete(Getresult.Id);
        }

        [Test]
        public async Task GetTeacherProfileExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userServices.ViewTeacherProfile(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

    }
}
