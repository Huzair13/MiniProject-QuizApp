using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models.DTOs.UserDTOs;
using QuizApp.Models;
using QuizApp.Repositories;
using QuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace QuizAppTest.RepositoryTest
{
    public class FillUpsRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        int LoggedInUser;

        IRepository<int, FillUps> fillUpsRepo;

        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        [SetUp]
        public async Task setup()
        {

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                        .UseInMemoryDatabase("FillUpsRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);

            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();


            fillUpsRepo = new FillUpsRepository(quizAppContext);

            IRepository<int, Teacher> teacherRepo = new TeacherRepository(quizAppContext);
            IRepository<int, Student> studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, User> userRepo = new UserRepository(quizAppContext);
            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);

            userLoginAndRegisterServices = new UserLoginAndRegisterServices
                                                   (userRepo, userDetailsRepo, 
                                                   tokenServices, teacherRepo, 
                                                   studentRepo, UserLoginAndRegisterMockLogger.Object);

            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Janu",
                Email = "janu@gmail.com",
                MobileNumber = "7687988998",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "janu123",
                Designation = "UG-Teacher",
                UserType = "Teacher"
            };

            //Action
            try
            {
                var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);
                LoggedInUser = result.Id;
            }
            catch (Exception ex) { }
        }

        [Test]
        public async Task AddFillupsSuccessTest()
        {
            //arrange
            FillUps question = new FillUps
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CorrectAnswer ="Answer"
            };

            //action
            var result = await fillUpsRepo.Add(question);

            //assert
            Assert.NotNull(result);
            await fillUpsRepo.Delete(result.Id);
        }

        [Test]
        public async Task UpdatFillUpsSuccessTest()
        {

            //arrange
            FillUps question = new FillUps
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CorrectAnswer = "Answer"
            };
            var result = await fillUpsRepo.Add(question);

            //Action
            var updatedResult = await fillUpsRepo.Update(result);

            //assert
            Assert.NotNull(updatedResult);
            await fillUpsRepo.Delete(result.Id);
        }

        [Test]
        public async Task UpdatFillUpsExceptionTest()
        {
            //arrange
            FillUps UpdateQuestion = new FillUps
            {
                Id = 999,
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CorrectAnswer = "Answer"
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await fillUpsRepo.Update(UpdateQuestion)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task DeleteFillupsSuccessTest()
        {
            //arrange
            FillUps question = new FillUps
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CorrectAnswer = "Answer"
            };
            var result = await fillUpsRepo.Add(question);

            //Action
            var DeleteResult = await fillUpsRepo.Delete(result.Id);

            //assert
            Assert.NotNull(DeleteResult);
            Assert.AreEqual(DeleteResult.Id, result.Id);
        }

        [Test]
        public async Task DeleteFillUpsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await fillUpsRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task GetFillUpsSuccessTest()
        {
            //arrange
            FillUps question = new FillUps
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CorrectAnswer = "Answer"
            };
            var result = await fillUpsRepo.Add(question);

            var questionResult = await fillUpsRepo.Get(result.Id);

            //assert
            Assert.NotNull(questionResult);
            Assert.AreEqual(questionResult.Id, result.Id);
            await fillUpsRepo.Delete(result.Id);
        }


        [Test]
        public async Task GetFillUpsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await fillUpsRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task GetAllFillUpsSuccessTest()
        {
            //arrange
            FillUps question = new FillUps
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                CorrectAnswer = "Answer"
            };

            var result = await fillUpsRepo.Add(question);

            var questionResult = await fillUpsRepo.Get();

            //assert
            Assert.NotNull(questionResult);
            Assert.NotZero(questionResult.Count());
            await fillUpsRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetAllFillUpsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await fillUpsRepo.Get()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }
    }
}
