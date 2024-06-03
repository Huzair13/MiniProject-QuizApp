using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.ResponseDTO;
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
    public class QuizResponseServiceTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IQuizResponseServices quizResponseServices;
        IQuizServices quizServices;
        IQuestionServices questionServices;
        Mock<ILogger<QuizServices>> QuizMockLogger;

        Mock<ILogger<QuestionServices>> mockLogger;
        Mock<ILogger<QuizResponseServices>> QuizResponseMockLogger;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        int LoggedInTeacher;
        int LoggedInStudent;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("ResponseServiceTestDB");

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
            QuizResponseMockLogger = new Mock<ILogger<QuizResponseServices>>();
            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();

            IRepository<int, Quiz> quizRepo = new QuizRepository(quizAppContext);
            IRepository<int, Question> questionRepository = new QuestionRepository(quizAppContext);
            IRepository<int, Response> responseRepo = new ResponseRepository(quizAppContext);
            IRepository<int, Student> studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, User> userRepo = new UserRepository(quizAppContext);
            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);
            IRepository<int, Teacher> teacherRepo = new TeacherRepository(quizAppContext);
            IRepository<int, FillUps> fillUpsRepo = new FillUpsRepository(quizAppContext);
            IRepository<int, MultipleChoice> multipleChoiceRepo = new MultipleChoiceRepository(quizAppContext);

            userLoginAndRegisterServices = new UserLoginAndRegisterServices
                                                   (userRepo, userDetailsRepo, 
                                                   tokenServices, teacherRepo, 
                                                   studentRepo, UserLoginAndRegisterMockLogger.Object);

            quizResponseServices = new QuizResponseServices(quizRepo, questionRepository, responseRepo, studentRepo, userRepo,QuizResponseMockLogger.Object);

            questionServices = new QuestionServices(questionRepository, fillUpsRepo, multipleChoiceRepo, teacherRepo, mockLogger.Object);
            quizServices = new QuizServices(quizRepo, questionRepository, teacherRepo, userRepo,QuizMockLogger.Object);


            await SeedData();

        }


        private async Task SeedData()
        {
            UserRegisterInputDTO teacher = new UserRegisterInputDTO
            {
                Name = "Sam",
                Email = "sam@gmail.com",
                MobileNumber = "7687988998",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "sam123",
                Designation = "UG-Teacher",
                UserType = "Teacher"
            };

            try
            {
                var result = await userLoginAndRegisterServices.Register(teacher);
                LoggedInTeacher = result.Id;
            }
            catch (Exception ex) { }

            UserRegisterInputDTO student = new UserRegisterInputDTO
            {
                Name = "Ram",
                Email = "ram@gmail.com",
                MobileNumber = "9898987766",
                DateOfBirth = new DateTime(2002 / 04 / 13),
                Password = "ram123",
                EducationQualification = "B.Tech - IT",
                UserType = "Student"
            };

            try
            {
                var result = await userLoginAndRegisterServices.Register(student);
                LoggedInStudent = result.Id;
            }

            catch (Exception ex) { }
        }

        [Test]
        public async Task StartQuizSuccessTest1()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuiz(mcqResult1.Id);

            // Act
            var result = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            // Assert
            Assert.NotNull(result);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task StartQuizSuccessTest2()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);

            // Action
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            var start2 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            // Assert
            Assert.NotNull(start2);
            Assert.AreEqual(start1.QuizName, start2.QuizName);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task StartQuizExceptionTest1()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuiz(mcqResult1.Id);

            // Action
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            var exception = Assert.ThrowsAsync<QuizAlreadyStartedException>(async () =>
                await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {QuizResult.QuizId} already started by you", exception.Message);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task StartQuizExceptionTest2()
        {
            //Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuiz(mcqResult1.Id);

            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.StartQuizAsync(LoggedInStudent, 999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerSuccessTest1()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { mcqResult1.Id, "Blue" } }
            };

            //Action
            var result = await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotZero(result.Count());
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerSuccessTest2()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { fillupsResult.Id, "Blue" } }
            };

            //Action
            var result = await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotZero(result.Count());
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerExceptionTest1()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { fillupsResult.Id, "Blue" } }
            };

            //Action
            var exception = Assert.ThrowsAsync<QuizNotStartedException>(async () =>
                await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {QuizResult.QuizId} not started", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }


        [Test]
        public async Task SubmitAllAnswerExceptionTest2()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            var startResult = quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { 999, "Blue" } }
            };

            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerExceptionTest3()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            var startResult = quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = 999,
                QuestionAnswers = new Dictionary<int, string>() { { fillupsResult.Id, "Blue" } }
            };

            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerExceptionTest4()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            var startResult = quizResponseServices.StartQuizAsync(999, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = 999,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { fillupsResult.Id, "Blue" } }
            };

            //Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerExceptionTest5()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = QuizResult.QuizId,
                TimeLimit = new TimeSpan(0, 0, 0)
            };

            var updateResult = await quizServices.EditQuizByIDAsync(quizUpdateDTO,LoggedInTeacher);

            var startResult = quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { fillupsResult.Id, "Blue" } }
            };

            //Action
            var exception = Assert.ThrowsAsync<QuizTimeLimitExceededException>(async () =>
                await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO)
            );

            // Assert
            Assert.AreEqual($"Sorry Quiz Time Limit has been exceeded", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }

        [Test]
        public async Task SubmitAllAnswerExceptionTest6()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            var startResult = quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAllAnswersDTO submitAllAnswersDTO = new SubmitAllAnswersDTO()
            {
                UserId = LoggedInStudent,
                QuizId = QuizResult.QuizId,
                QuestionAnswers = new Dictionary<int, string>() { { fillupsResult.Id, "Blue" } }
            };

            var submitResult = quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO);

            //Action
            var exception = Assert.ThrowsAsync<UserAlreadyAnsweredTheQuestionException>(async () =>
                await quizResponseServices.SubmitAllAnswersAsync(submitAllAnswersDTO)
            );

            // Assert
            Assert.AreEqual($"User has already answered question with ID {fillupsResult.Id}.", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }

        [Test]
        public async Task SubmitAnswerSuccessTest1()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = mcqResult1.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };

            //Action
            var result = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotZero(result.Count());
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task SubmitAnswerSuccessTest2()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = mcqResult1.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Oramge"
            };

            //Action
            var result = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotZero(result.Count());
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task SubmitAnswerSuccessTest3()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillUps.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Oramge"
            };

            //Action
            var result = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            // Assert
            Assert.NotNull(result);
            Assert.NotZero(result.Count());
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }

        [Test]
        public async Task SubmitAnswerExceptionTest1()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = mcqResult1.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };
            var submitResult = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            //Action
            var exception = Assert.ThrowsAsync<UserAlreadyAnsweredTheQuestionException>(async () =>
                await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO)
            );

            // Assert
            Assert.AreEqual($"User Already Asnwered the Question", exception.Message);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task SubmitAnswerExceptionTest2()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = mcqResult1.Id,
                QuizId = 999,
                Answer = "Blue"
            };

            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }


        [Test]
        public async Task SubmitAnswerExceptionTest3()
        {
            // Arrange
            var mcqResult1 = await createMCQ();
            var QuizResult = await createQuizAllowMultiple(mcqResult1.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = 999,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };

            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO)
            );

            // Assert
            Assert.AreEqual($"Question with ID {999} is not part of the quiz with ID {QuizResult.QuizId}.", exception.Message);
            await DeleteMCQAndQuiz(mcqResult1, QuizResult);
        }

        [Test]
        public async Task SubmitAnswerExceptionTest4()
        {
            // Arrange
            var fillupsResult = await createFillUps();
            var QuizResult = await createQuiz(fillupsResult.Id);

            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = QuizResult.QuizId,
                TimeLimit = new TimeSpan(0, 0, 0)
            };

            var updateResult = await quizServices.EditQuizByIDAsync(quizUpdateDTO, LoggedInTeacher);

            var startResult = quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillupsResult.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };

            //Action
            var exception = Assert.ThrowsAsync<QuizTimeLimitExceededException>(async () =>
                await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO)
            );

            // Assert
            Assert.AreEqual($"Sorry Quiz Time Limit has been exceeded", exception.Message);
            await DeleteFillUpsAndQuiz(fillupsResult, QuizResult);
        }


        [Test]
        public async Task GetLeaderBoardSuccessTest()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillUps.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };
            var submitResult = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            //Action
            var leaderBoard = await quizResponseServices.GetQuizLeaderboardAsync(QuizResult.QuizId);

            // Assert
            Assert.NotNull(leaderBoard);
            Assert.NotZero(leaderBoard.Count());
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }

        [Test]
        public async Task GetLeaderBoardExceptionTest()
        {
            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.GetQuizLeaderboardAsync(999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }


        [Test]
        public async Task GetStudentLeaderBoardSuccessTest()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillUps.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };
            var submitResult = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            //Action
            var leaderBoard = await quizResponseServices.GetStudentQuizLeaderboardAsync(QuizResult.QuizId);

            // Assert
            Assert.NotNull(leaderBoard);
            Assert.NotZero(leaderBoard.Count());
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }

        [Test]
        public async Task GetStudentLeaderBoardExceptionTest()
        {
            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.GetStudentQuizLeaderboardAsync(999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task GetStudentPositionSuccessTest1()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillUps.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };
            var submitResult = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            //Action
            var position = await quizResponseServices.GetStudentPositionInLeaderboardAsync(LoggedInStudent, QuizResult.QuizId);

            // Assert
            Assert.NotNull(position);
            Assert.AreEqual(position, 1);
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }


        [Test]
        public async Task GetStudentPositionSuccessTest2()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);

            //Action
            var position = await quizResponseServices.GetStudentPositionInLeaderboardAsync(LoggedInStudent, QuizResult.QuizId);

            // Assert
            Assert.NotNull(position);
            Assert.AreEqual(position, -1);
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }


        [Test]
        public async Task GetStudentPositionExceptionTest1()
        {
            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.GetStudentPositionInLeaderboardAsync(LoggedInStudent, 999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }

        public async Task GetStudentPositionExceptionTest2()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);

            //Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await quizResponseServices.GetStudentPositionInLeaderboardAsync(999, QuizResult.QuizId)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetAllStudentResponseSuccessTest()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillUps.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };
            var submitResult = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            //Action
            var response = await quizResponseServices.GetAllUseresponsesAsync(LoggedInStudent);

            // Assert
            Assert.NotNull(response);
            Assert.NotZero(response.Count());
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }

        [Test]
        public async Task GetAllStudentResponseExceptionTest()
        {

            //Action
            var exception = Assert.ThrowsAsync<NoSuchResponseException>(async () =>
                await quizResponseServices.GetAllUseresponsesAsync(999)
            );

            // Assert
            Assert.AreEqual($"Response Not Found", exception.Message);
        }

        [Test]
        public async Task GetQuizResultSuccessTest()
        {
            // Arrange
            var fillUps = await createFillUps();
            var QuizResult = await createQuizAllowMultiple(fillUps.Id);
            var start1 = await quizResponseServices.StartQuizAsync(LoggedInStudent, QuizResult.QuizId);

            SubmitAnswerDTO submitAnswerDTO = new SubmitAnswerDTO()
            {
                UserId = LoggedInStudent,
                QuestionId = fillUps.Id,
                QuizId = QuizResult.QuizId,
                Answer = "Blue"
            };
            var submitResult = await quizResponseServices.SubmitAnswerAsync(submitAnswerDTO);

            //Action
            var response = await quizResponseServices.GetQuizResultAsync(LoggedInStudent, QuizResult.QuizId);

            // Assert
            Assert.NotNull(response);
            Assert.NotZero(response.Count());
            await DeleteFillUpsAndQuiz(fillUps, QuizResult);
        }

        [Test]
        public async Task GetQuizResultExceptionTest()
        {
            //Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizResponseServices.GetQuizResultAsync(LoggedInStudent, 999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }






        private async Task DeleteFillUpsAndQuiz(FillUpsReturnDTO fillupsResult, QuizReturnDTO quizResult)
        {
            await questionServices.DeleteQuestionByID(fillupsResult.Id, LoggedInTeacher);
            await quizServices.DeleteQuizByIDAsync(quizResult.QuizId, LoggedInTeacher);
        }

        private async Task DeleteMCQAndQuiz(QuestionReturnDTO mcqResult1, QuizReturnDTO QuizResult)
        {
            await questionServices.DeleteQuestionByID(mcqResult1.Id, LoggedInTeacher);
            await quizServices.DeleteQuizByIDAsync(QuizResult.QuizId, LoggedInTeacher);
        }

        private async Task<QuizReturnDTO> createQuizAllowMultiple(int questiondId)
        {
            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInTeacher,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                TimeLimit = new TimeSpan(0,20,0),
                QuestionIds = new List<int> { questiondId }
            };
            var QuizResult = await quizServices.AddQuizAsync(quizDTO);
            return QuizResult;
        }

        private async Task<QuizReturnDTO> createQuiz(int questionId)
        {
            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInTeacher,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = false,
                TimeLimit = new TimeSpan(0, 20, 0),
                QuestionIds = new List<int> { questionId }

            };
            var QuizResult = await quizServices.AddQuizAsync(quizDTO);
            return QuizResult;
        }

        private async Task<FillUpsReturnDTO> createFillUps()
        {
            FillUpsDTO fillUps1 = new FillUpsDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Points = 10,
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                CorrectAnswer = "Blue",
                CreatedDate = DateTime.Now,
                CreatedBy = LoggedInTeacher,
                QuestionType = "GK"
            };
            var fillUpsResult1 = await questionServices.AddFillUpsQuestion(fillUps1);
            return fillUpsResult1;
        }
        private async Task<QuestionReturnDTO> createMCQ()
        {
            // Arrange
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
                CreatedBy = LoggedInTeacher,
                QuestionType = "GK"
            };
            var mcqResult1 = await questionServices.AddMCQQuestion(mcq1);
            return mcqResult1;
        }
    }
}
