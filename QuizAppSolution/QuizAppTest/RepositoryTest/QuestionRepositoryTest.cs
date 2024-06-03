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

namespace QuizAppTest.RepositoryTest
{
    public class QuestionRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        int LoggedInUser;

        IRepository<int, Question> questionRepository;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        [SetUp]
        public async Task setup()
        {
            
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                        .UseInMemoryDatabase("QuestionRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);

            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();


            questionRepository = new QuestionRepository(quizAppContext);

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
        public async Task AddQuestionSuccessTest()
        {
            //arrange
            Question question = new MultipleChoice
            {
                QuestionText = "Sample",
                QuestionType = "MultipleChoice",
                Category = "Test",
                DifficultyLevel = DifficultyLevel.Medium,
                QuestionCreatedBy = LoggedInUser,
                Points = 10,
                IsDeleted = false,
                CreatedDate = DateTime.Now,
                Choice1="c1",
                Choice2="c2",
                Choice3 = "c3",
                Choice4 ="c4",
                CorrectChoice ="c1"
            };

            //action
            var result = await questionRepository.Add(question);

            //assert
            Assert.NotNull(result);
            await questionRepository.Delete(result.Id);
        }

        [Test]
        public async Task UpdatQuestionSuccessTest()
        {

            //arrange
            Question question = new MultipleChoice
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
            var result = await questionRepository.Add(question);

            //Action
            var updatedResult = await questionRepository.Update(result);

            //assert
            Assert.NotNull(updatedResult);
            await questionRepository.Delete(result.Id);
        }

        [Test]
        public async Task UpdatQuestionExceptionTest()
        {
            //arrange
            Question UpdateQuestion = new Question
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
                await questionRepository.Update(UpdateQuestion)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task DeleteQuestionSuccessTest()
        {
            //arrange
            Question question = new MultipleChoice
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
            var result = await questionRepository.Add(question);

            //Action
            var DeleteResult = await questionRepository.Delete(result.Id);

            //assert
            Assert.NotNull(DeleteResult);
            Assert.AreEqual(DeleteResult.Id, result.Id);
        }

        [Test]
        public async Task DeleteQuestionExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionRepository.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task GetQuestionSuccessTest()
        {
            //arrange
            Question question = new MultipleChoice
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
            var result = await questionRepository.Add(question);

            var questionResult = await questionRepository.Get(result.Id);

            //assert
            Assert.NotNull(questionResult);
            Assert.AreEqual(questionResult.Id, result.Id);
            await questionRepository.Delete(result.Id);
        }


        [Test]
        public async Task GetQuestionExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionRepository.Get(999)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task GetAllQuestionSuccessTest()
        {
            //arrange
            Question question = new MultipleChoice
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

            var result = await questionRepository.Add(question);

            var questionResult = await questionRepository.Get();

            //assert
            Assert.NotNull(questionResult);
            Assert.NotZero(questionResult.Count());
            await questionRepository.Delete(result.Id);
        }

        [Test]
        public async Task GetAllQuestionFailureTest()
        {
            // Action
            var questionResult = await questionRepository.Get();

            //assert
            Assert.IsEmpty(questionResult);
        }
    }
}
