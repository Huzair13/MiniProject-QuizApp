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
    public class MCQRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        int LoggedInUser;
        IRepository<int, MultipleChoice> mcqRepos;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        [SetUp]
        public async Task setup()
        {

            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                        .UseInMemoryDatabase("MCQRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);

            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();


            mcqRepos = new MultipleChoiceRepository(quizAppContext);

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
        public async Task AddMCQSuccessTest()
        {
            //arrange
            MultipleChoice question = new MultipleChoice
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Choice1 = "c1",
                Choice2 = "c2",
                Choice3 = "c3",
                Choice4 = "c4",
                CorrectChoice = "c1"
            };

            //action
            var result = await mcqRepos.Add(question);

            //assert
            Assert.NotNull(result);
            await mcqRepos.Delete(result.Id);
        }

        [Test]
        public async Task UpdatMCQSuccessTest()
        {

            //arrange
            MultipleChoice question = new MultipleChoice
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Choice1 = "c1",
                Choice2 = "c2",
                Choice3 = "c3",
                Choice4 = "c4",
                CorrectChoice = "c1"
            };
            var result = await mcqRepos.Add(question);

            //Action
            var updatedResult = await mcqRepos.Update(result);

            //assert
            Assert.NotNull(updatedResult);
            await mcqRepos.Delete(result.Id);
        }

        [Test]
        public async Task UpdatMCQExceptionTest()
        {
            //arrange
            MultipleChoice UpdateQuestion = new MultipleChoice
            {
                Id = 999,
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Easy,
                QuestionCreatedBy = LoggedInUser,
                Points = 15,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await mcqRepos.Update(UpdateQuestion)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task DeleteMCQSuccessTest()
        {
            //arrange
            MultipleChoice question = new MultipleChoice
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Choice1 = "c1",
                Choice2 = "c2",
                Choice3 = "c3",
                Choice4 = "c4",
                CorrectChoice = "c1"
            };
            var result = await mcqRepos.Add(question);

            //Action
            var DeleteResult = await mcqRepos.Delete(result.Id);

            //assert
            Assert.NotNull(DeleteResult);
            Assert.AreEqual(DeleteResult.Id, result.Id);
        }

        [Test]
        public async Task DeleteMCQExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await mcqRepos.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task GetMCQSuccessTest()
        {
            //arrange
            MultipleChoice question = new MultipleChoice
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Choice1 = "c1",
                Choice2 = "c2",
                Choice3 = "c3",
                Choice4 = "c4",
                CorrectChoice = "c1"
            };
            var result = await mcqRepos.Add(question);

            var questionResult = await mcqRepos.Get(result.Id);

            //assert
            Assert.NotNull(questionResult);
            Assert.AreEqual(questionResult.Id, result.Id);
            await mcqRepos.Delete(result.Id);
        }


        [Test]
        public async Task GetMCQExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await mcqRepos.Get(999)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task GetAllMCQSuccessTest()
        {
            //arrange
            MultipleChoice question = new MultipleChoice
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Choice1 = "c1",
                Choice2 = "c2",
                Choice3 = "c3",
                Choice4 = "c4",
                CorrectChoice = "c1"
            };

            var result = await mcqRepos.Add(question);

            var questionResult = await mcqRepos.Get();

            //assert
            Assert.NotNull(questionResult);
            Assert.NotZero(questionResult.Count());
            await mcqRepos.Delete(result.Id);
        }

        [Test]
        public async Task GetAllMCQExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await mcqRepos.Get()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }
    }
}
