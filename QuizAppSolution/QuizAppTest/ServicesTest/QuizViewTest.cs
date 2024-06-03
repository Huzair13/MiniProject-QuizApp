using QuizApp.Exceptions;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models;
using QuizApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using QuizApp.Contexts;
using QuizApp.Interfaces;
using QuizApp.Models.DTOs.UserDTOs;
using QuizApp.Repositories;
using QuizApp.Models.DTOs.MCQDTOs;
using Microsoft.Extensions.Logging;

namespace QuizAppTest.ServicesTest
{
    public class QuizViewTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IQuizServices quizServices;

        IQuestionServices questionServices;
        IRepository<int, User> userRepo;
        IRepository<int, Teacher> teacherRepo;

        Mock<ILogger<QuestionServices>> mockLogger;
        Mock<ILogger<QuizServices>> QuizMockLogger;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        int LoggedInUser;
        int questionID;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("QuizRepoViewTestDB");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);

            mockLogger = new Mock<ILogger<QuestionServices>>();
            QuizMockLogger = new Mock<ILogger<QuizServices>>();
            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();

            IRepository<int, Question> QuestionRepository = new QuestionRepository(quizAppContext);
            IRepository<int, FillUps> fillUpsRepo = new FillUpsRepository(quizAppContext);
            IRepository<int, MultipleChoice> multipleChoiceRepo = new MultipleChoiceRepository(quizAppContext);
            teacherRepo = new TeacherRepository(quizAppContext);
            IRepository<int, Student> studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, Quiz> quizRepo = new QuizRepository(quizAppContext);
            userRepo = new UserRepository(quizAppContext);
            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);
            userLoginAndRegisterServices = new UserLoginAndRegisterServices
            (userRepo, userDetailsRepo, tokenServices, teacherRepo, studentRepo,UserLoginAndRegisterMockLogger.Object);

            quizServices = new QuizServices(quizRepo, QuestionRepository, teacherRepo, userRepo, QuizMockLogger.Object);
            questionServices = new QuestionServices(QuestionRepository, fillUpsRepo, multipleChoiceRepo, teacherRepo,mockLogger.Object);


            UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
            {
                Name = "Sam",
                Email = "sam@gmail.com",
                MobileNumber = "7687988998",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "sam123",
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

            if ((await QuestionRepository.Get()).Count() == 0)
            {
                MCQDTO mcq = new MCQDTO()
                {
                    QuestionText = "What is th color of Sky ?",
                    Points = 10,
                    Category = "General",
                    DifficultyLevel = DifficultyLevel.Medium,
                    Choice1 = "Yellow",
                    Choice2 = "Blue",
                    Choice3 = "Orange",
                    Choice4 = "Red",
                    CorrectAnswer = "Blue",
                    CreatedDate = DateTime.Now,
                    CreatedBy = LoggedInUser,
                    QuestionType = "GK"
                };
                var result = await questionServices.AddMCQQuestion(mcq);
                questionID = result.Id;
            }
        }

        [Test]
        public async Task GetAllQuizSuccessTest()
        {
            //arrange
            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInUser,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            var addResult = await quizServices.AddQuizAsync(quizDTO);

            var quizzes = await quizServices.GetAllQuizzesAsync();

            // Assert
            Assert.NotZero(quizzes.Count());
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task GetAllQuizExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.GetAllQuizzesAsync()
            );

            // Assert
            Assert.AreEqual($"Quiz Not Found", exception.Message);
        }

        [Test]
        public async Task GetAllSoftDeletedQuizSuccessTest()
        {
            //arrange
            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInUser,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            var addResult = await quizServices.AddQuizAsync(quizDTO);
            var delQuiz = await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            var Deletedquizzes = await quizServices.GetAllSoftDeletedQuizzesAsync();

            // Assert
            Assert.NotZero(Deletedquizzes.Count());
            await quizServices.UndoSoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task GetAllSoftDeletedQuizExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.GetAllSoftDeletedQuizzesAsync()
            );

            // Assert
            Assert.AreEqual($"Quiz Not Found", exception.Message);
        }

        [Test]
        public async Task GetAllQuizzesCreatedByLoggedInTeacherSuccessTest()
        {
            //arrange
            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInUser,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            var addResult = await quizServices.AddQuizAsync(quizDTO);

            //action
            var quizzes = await quizServices.GetAllQuizzesCreatedByLoggedInTeacherAsync(LoggedInUser);

            // Assert
            Assert.NotZero(quizzes.Count());
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task GetAllQuizzesCreatedByLoggedInTeacherExceptionTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.GetAllQuizzesCreatedByLoggedInTeacherAsync(999)
            );

            // Assert
            Assert.AreEqual($"Quiz Not Found", exception.Message);
        }
    }
}
