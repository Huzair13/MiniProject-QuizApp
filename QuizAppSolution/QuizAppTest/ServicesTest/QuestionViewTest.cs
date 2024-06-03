using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using QuizApp.Contexts;
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
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Exceptions;
using Microsoft.Extensions.Logging;

namespace QuizAppTest.ServicesTest
{
    public class QuestionViewTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IQuestionServices questionServices;
        IQuestionViewServices questionViewServices;

        Mock<ILogger<QuestionViewServices>> QviewMockLogger;
        Mock<ILogger<QuestionServices>> mockLogger;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        int LoggedInUser;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("QuizAppDB");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            
            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();

            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);

            mockLogger = new Mock<ILogger<QuestionServices>>();
            QviewMockLogger = new Mock<ILogger<QuestionViewServices>>();
            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();

            IRepository<int, Question> QuestionRepository = new QuestionRepository(quizAppContext);
            IRepository<int, FillUps> fillUpsRepo = new FillUpsRepository(quizAppContext);
            IRepository<int, MultipleChoice> multipleChoiceRepo = new MultipleChoiceRepository(quizAppContext);
            IRepository<int, Teacher> teacherRepo = new TeacherRepository(quizAppContext);
            IRepository<int, Student> studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, User> userRepo = new UserRepository(quizAppContext);
            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);

            userLoginAndRegisterServices = new UserLoginAndRegisterServices
                                                   (userRepo, userDetailsRepo, tokenServices, 
                                                   teacherRepo, studentRepo, UserLoginAndRegisterMockLogger.Object);
            questionServices = new QuestionServices(QuestionRepository, fillUpsRepo, multipleChoiceRepo, teacherRepo,mockLogger.Object);
            questionViewServices = new QuestionViewServices(QuestionRepository,QviewMockLogger.Object);




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
        public async Task GetAllFillUpsSuccessTest()
        {
            //Arrange
            FillUpsDTO fillUps1 = new FillUpsDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Points = 10,
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                CorrectAnswer = "Blue",
                CreatedDate = DateTime.Now,
                CreatedBy = LoggedInUser,
                QuestionType = "GK"
            };
            var fillUpsResult1 = await questionServices.AddFillUpsQuestion(fillUps1);
            //Action
            var results = await questionViewServices.GetAllFillUpsQuestionsAsync();

            //Assert
            Assert.NotNull(results);
            await questionServices.DeleteQuestionByID(fillUpsResult1.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllFillUpsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllFillUpsQuestionsAsync()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }

        [Test]
        public async Task GetAllMCQSuccessTest()
        {
            //Arrange
            FillUpsDTO fillUps1 = new FillUpsDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Points = 10,
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                CorrectAnswer = "Blue",
                CreatedDate = DateTime.Now,
                CreatedBy = LoggedInUser,
                QuestionType = "GK"
            };
            var fillUpsResult1 = await questionServices.AddFillUpsQuestion(fillUps1);

            MCQDTO mcq1 = new MCQDTO()
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
            var mcqResult1 = await questionServices.AddMCQQuestion(mcq1);

            MCQDTO mcq2 = new MCQDTO()
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
            var mcqResult2 = await questionServices.AddMCQQuestion(mcq2);

            var results = await questionViewServices.GetAllMCQQuestionsAsync();

            //Assert
            Assert.NotNull(results);
            await questionServices.DeleteQuestionByID(mcqResult1.Id, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult2.Id, LoggedInUser);
            await questionServices.DeleteQuestionByID(fillUpsResult1.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllMCQExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllMCQQuestionsAsync()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }

        [Test]
        public async Task GetAllQuestionsSuccessTest()
        {
            // Arrange: Create and add various questions
            MCQDTO mcq1 = new MCQDTO()
            {
                QuestionText = "What is the color of the sky?",
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

            FillUpsDTO fillUp1 = new FillUpsDTO()
            {
                QuestionText = "What is 2 + 2?",
                Points = 5,
                Category = "Math",
                DifficultyLevel = DifficultyLevel.Easy,
                CorrectAnswer = "4",
                CreatedDate = DateTime.Now,
                CreatedBy = LoggedInUser,
                QuestionType = "Math"
            };
            var mcqResult1 = await questionServices.AddMCQQuestion(mcq1);
            var fillUpResult1 = await questionServices.AddFillUpsQuestion(fillUp1);

            // Action
            var results = await questionViewServices.GetAllQuestionsAsync();

            // Assert
            Assert.NotZero(results.Count());
            await questionServices.DeleteQuestionByID(mcqResult1.Id, LoggedInUser);
            await questionServices.DeleteQuestionByID(fillUpResult1.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllQuestionsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllQuestionsAsync()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }


        [Test]
        public async Task GetAllSoftDeletedQuestionsSuccessTest()
        {
            //arrange
            MCQDTO mcq2 = new MCQDTO()
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
            var mcqResult2 = await questionServices.AddMCQQuestion(mcq2);
            var delResult = await questionServices.SoftDeleteQuestionByIDAsync(mcqResult2.Id, LoggedInUser);

            //Action
            var results = await questionViewServices.GetAllSoftDeletedQuestionsAsync();

            //Assert
            Assert.AreEqual(results.First().Id, delResult.Id);
        }

        [Test]
        public async Task GetAllSoftDeletedQuestionsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllSoftDeletedQuestionsAsync()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }

        [Test]
        public async Task GetAllQuestionsByLoggedUserSuccessTest()
        {
            //arrange
            MCQDTO mcq2 = new MCQDTO()
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
            var mcqResult2 = await questionServices.AddMCQQuestion(mcq2);

            //Action
            var results = await questionViewServices.GetAllQuestionsCreatedByLoggedInTeacherAsync(LoggedInUser);

            //Assert
            Assert.AreEqual(results.First().QuestionCreatedBy, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult2.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllQuestionsByLoggedUserExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllQuestionsCreatedByLoggedInTeacherAsync(LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }


        [Test]
        public async Task GetAllHardQuestionSuccessTest()
        {
            //arrange
            MCQDTO mcq2 = new MCQDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Points = 10,
                Category = "General",
                DifficultyLevel = DifficultyLevel.Hard,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Blue",
                CreatedDate = DateTime.Now,
                CreatedBy = LoggedInUser,
                QuestionType = "GK"
            };
            var mcqResult2 = await questionServices.AddMCQQuestion(mcq2);

            //Action
            var results = await questionViewServices.GetAllHardQuestions();

            //Assert
            Assert.AreEqual(results.First().QuestionCreatedBy, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult2.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllHardQuestionExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllHardQuestions()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }


        [Test]
        public async Task GetAllMediumQuestionSuccessTest()
        {
            //arrange
            MCQDTO mcq2 = new MCQDTO()
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
            var mcqResult2 = await questionServices.AddMCQQuestion(mcq2);

            //Action
            var results = await questionViewServices.GetAllMediumQuestions();

            //Assert
            Assert.AreEqual(results.First().QuestionCreatedBy, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult2.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllMediumQuestionExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllMediumQuestions()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }

        [Test]
        public async Task GetAllEasyQuestionSuccessTest()
        {
            //arrange
            MCQDTO mcq2 = new MCQDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Points = 10,
                Category = "General",
                DifficultyLevel = DifficultyLevel.Easy,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Blue",
                CreatedDate = DateTime.Now,
                CreatedBy = LoggedInUser,
                QuestionType = "GK"
            };
            var mcqResult2 = await questionServices.AddMCQQuestion(mcq2);

            //Action
            var results = await questionViewServices.GetAllEasyQuestions();

            //Assert
            Assert.AreEqual(results.First().QuestionCreatedBy, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult2.Id, LoggedInUser);
        }

        [Test]
        public async Task GetAllEasyQuestionExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionViewServices.GetAllEasyQuestions()
            );

            // Assert
            Assert.AreEqual($"Question Not Found", exception.Message);
        }
    }
}
