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
using QuizApp.Models.DTOs.QuizDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
using QuizApp.Exceptions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.Extensions.Logging;

namespace QuizAppTest.ServicesTest
{
    public class QuizServiceTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IQuizServices quizServices;
        IQuestionServices questionServices;

        Mock<ILogger<QuestionServices>> mockLogger;
        Mock<ILogger<QuizServices>> QuizMockLogger;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        int LoggedInUser;
        int questionID;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("QuizRepoTestDB");

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
            IRepository<int, Teacher> teacherRepo = new TeacherRepository(quizAppContext);
            IRepository<int, Student> studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, Quiz> quizRepo = new QuizRepository(quizAppContext);
            IRepository<int, User> userRepo = new UserRepository(quizAppContext);
            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);
            userLoginAndRegisterServices = new UserLoginAndRegisterServices
                                                   (userRepo, userDetailsRepo, 
                                                   tokenServices, teacherRepo, 
                                                   studentRepo, UserLoginAndRegisterMockLogger.Object);

            quizServices = new QuizServices(quizRepo, QuestionRepository, teacherRepo, userRepo,QuizMockLogger.Object);
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
        public async Task AddQuizSuccessTest()
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
            //action
            var result = await quizServices.AddQuizAsync(quizDTO);

            //assert
            Assert.AreEqual(result.QuizName, quizDTO.QuizName);
            await quizServices.DeleteQuizByIDAsync(result.QuizId, LoggedInUser);
        }

        [Test]
        public async Task AddQuizExceptionTest()
        {
            //arrange
            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = 999,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await quizServices.AddQuizAsync(quizDTO)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {quizDTO.QuizCreatedBy}", exception.Message);
        }

        [Test]
        public async Task EditQuizByIDSuccessTest()
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

            var result = await quizServices.AddQuizAsync(quizDTO);
            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = result.QuizId,
                QuizName = "Test Updated",
                QuizType = result.QuizType,
                IsMultipleAttemptAllowed = false,
                QuestionIds = new List<int> { questionID },
                QuizDescription = result.QuizDescription
            };

            //ACTION
            var updatedQuiz = await quizServices.EditQuizByIDAsync(quizUpdateDTO, LoggedInUser);

            //ASSERT
            Assert.AreEqual(updatedQuiz.QuizName, quizUpdateDTO.QuizName);
            await quizServices.DeleteQuizByIDAsync(result.QuizId, LoggedInUser);
        }

        [Test]
        public async Task EditQuizByIDExceptionTest1()
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

            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = 999,
                QuizName = "Test Updated",
                QuizType = addResult.QuizType,
                IsMultipleAttemptAllowed = false,
                QuestionIds = new List<int> { questionID },
                QuizDescription = addResult.QuizDescription
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.EditQuizByIDAsync(quizUpdateDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {quizUpdateDTO.QuizID} not found", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task EditQuizByIDExceptionTest2()
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

            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = addResult.QuizId,
                QuizName = "Test Updated",
                QuizType = addResult.QuizType,
                IsMultipleAttemptAllowed = false,
                QuestionIds = new List<int> { questionID },
                QuizDescription = addResult.QuizDescription
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await quizServices.EditQuizByIDAsync(quizUpdateDTO, 999)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task EditQuizByIDExceptionTest3()
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
            var deleteResult = await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = addResult.QuizId,
                QuizName = "Test Updated",
                QuizType = addResult.QuizType,
                IsMultipleAttemptAllowed = false,
                QuestionIds = new List<int> { questionID },
                QuizDescription = addResult.QuizDescription
            };

            // Action
            var exception = Assert.ThrowsAsync<QuizDeletedException>(async () =>
                await quizServices.EditQuizByIDAsync(quizUpdateDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Given Quiz Has been Deleted by You already", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task EditQuizByIDExceptionTest4()
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

            QuizUpdateDTO quizUpdateDTO = new QuizUpdateDTO()
            {
                QuizID = addResult.QuizId,
                QuizName = "Test Updated",
                QuizType = addResult.QuizType,
                IsMultipleAttemptAllowed = false,
                QuestionIds = new List<int> { 10 },
                QuizDescription = addResult.QuizDescription
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await quizServices.EditQuizByIDAsync(quizUpdateDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {10}", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task DeleteQuizByIDSuccessTest()
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

            // Action
            var delResult = await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            // Assert
            Assert.AreEqual(delResult.QuizId, addResult.QuizId);

        }


        [Test]
        public async Task DeleteQuizByIDExceptionTest1()
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


            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToDeleteException>(async () =>
                await quizServices.DeleteQuizByIDAsync(addResult.QuizId, 999)
            );

            // Assert
            Assert.AreEqual($"You cant delete this. Only Creator can Delete", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task DeleteQuizByIDExceptionTest2()
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


            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.DeleteQuizByIDAsync(999, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task SoftDeleteQuizByIDSuccessTest()
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

            // Action
            var delResult = await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            // Assert
            Assert.AreEqual(delResult.QuizId, addResult.QuizId);
            await quizServices.DeleteQuizByIDAsync(delResult.QuizId, LoggedInUser);

        }


        [Test]
        public async Task SoftDeleteQuizByIDExceptionTest1()
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


            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToDeleteException>(async () =>
                await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, 999)
            );

            // Assert
            Assert.AreEqual($"You cant delete this. Only Creator can Delete", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task SoftDeleteQuizByIDExceptionTest2()
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


            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.SoftDeleteQuizByIDAsync(999, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task UndoSoftDeleteQuizByIDSuccessTest()
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
            var delResult = await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            var undoDelResult = await quizServices.UndoSoftDeleteQuizByIDAsync(delResult.QuizId, LoggedInUser);

            // Assert
            Assert.AreEqual(delResult.QuizId, undoDelResult.QuizId);
            await quizServices.DeleteQuizByIDAsync(delResult.QuizId, LoggedInUser);

        }


        [Test]
        public async Task UndoSoftDeleteQuizByIDExceptionTest1()
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
            var delResult = await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await quizServices.UndoSoftDeleteQuizByIDAsync(delResult.QuizId, 999)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task UndoSoftDeleteQuizByIDExceptionTest2()
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
            var delResult = await quizServices.SoftDeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.SoftDeleteQuizByIDAsync(999, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task CreateQuizFromExistingQuizSuccessTest()
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
            var newQuiz = await quizServices.CreateQuizFromExistingQuiz(addResult.QuizId, LoggedInUser);

            // Assert
            Assert.AreEqual(newQuiz.QuizName, addResult.QuizName);
            Assert.AreNotEqual(newQuiz.QuizId, addResult.QuizId);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task CreateQuizFromExistingQuizExceptionTest1()
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

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await quizServices.CreateQuizFromExistingQuiz(addResult.QuizId, 999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task CreateQuizFromExistingQuizExceptionTest2()
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

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.CreateQuizFromExistingQuiz(998, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {998} not found", exception.Message);
            await quizServices.DeleteQuizByIDAsync(addResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task AddQuestionsToQuizSuccessTest1()
        {
            MCQDTO mcq = new MCQDTO()
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
            var mcqResult = await questionServices.AddMCQQuestion(mcq);

            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInUser,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            var quizResult = await quizServices.AddQuizAsync(quizDTO);

            //arrange
            AddQuestionsToQuizDTO addQuestionsToQuizDTO = new AddQuestionsToQuizDTO()
            {
                QuizId = quizResult.QuizId,
                QuestionIds = new List<int>() { mcqResult.Id }
            };


            //action
            var newQuiz = await quizServices.AddQuestionsToQuizAsync(addQuestionsToQuizDTO, LoggedInUser);

            // Assert
            Assert.AreEqual(newQuiz.QuizQuestions.Count(), 2);
            await quizServices.DeleteQuizByIDAsync(quizResult.QuizId, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult.Id, LoggedInUser);
        }

        [Test]
        public async Task AddQuestionsToQuizSuccessTest2()
        {
            MCQDTO mcq = new MCQDTO()
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
            var mcqResult = await questionServices.AddMCQQuestion(mcq);

            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInUser,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            var quizResult = await quizServices.AddQuizAsync(quizDTO);

            //arrange
            AddQuestionsToQuizDTO addQuestionsToQuizDTO = new AddQuestionsToQuizDTO()
            {
                QuizId = quizResult.QuizId,
                QuestionIds = new List<int>() { mcqResult.Id, questionID }
            };


            //action
            var newQuiz = await quizServices.AddQuestionsToQuizAsync(addQuestionsToQuizDTO, LoggedInUser);

            // Assert
            Assert.AreEqual(newQuiz.QuizQuestions.Count(), 2);
            await quizServices.DeleteQuizByIDAsync(quizResult.QuizId, LoggedInUser);
            await questionServices.DeleteQuestionByID(mcqResult.Id, LoggedInUser);
        }

        [Test]
        public async Task AddQuestionsToQuizExceptionTest1()
        {
            MCQDTO mcq = new MCQDTO()
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
            var mcqResult = await questionServices.AddMCQQuestion(mcq);

            //arrange
            AddQuestionsToQuizDTO addQuestionsToQuizDTO = new AddQuestionsToQuizDTO()
            {
                QuizId = 356,
                QuestionIds = new List<int>() { mcqResult.Id }
            };


            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizServices.AddQuestionsToQuizAsync(addQuestionsToQuizDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {356} not found", exception.Message);
            await questionServices.DeleteQuestionByID(mcqResult.Id, LoggedInUser);
        }

        [Test]
        public async Task AddQuestionsToQuizExceptionTest2()
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

            var quizResult = await quizServices.AddQuizAsync(quizDTO);

            AddQuestionsToQuizDTO addQuestionsToQuizDTO = new AddQuestionsToQuizDTO()
            {
                QuizId = quizResult.QuizId,
                QuestionIds = new List<int>() { 567 }
            };


            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await quizServices.AddQuestionsToQuizAsync(addQuestionsToQuizDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {567}", exception.Message);
            await quizServices.DeleteQuizByIDAsync(quizResult.QuizId, LoggedInUser);
        }

        [Test]
        public async Task AddQuestionsToQuizExceptionTest3()
        {

            //arrange

            MCQDTO mcq = new MCQDTO()
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
            var mcqResult = await questionServices.AddMCQQuestion(mcq);

            QuizDTO quizDTO = new QuizDTO()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = LoggedInUser,
                QuizDescription = "Sample Test",
                IsMultpleAttemptAllowed = true,
                QuestionIds = new List<int> { questionID }
            };

            var quizResult = await quizServices.AddQuizAsync(quizDTO);

            AddQuestionsToQuizDTO addQuestionsToQuizDTO = new AddQuestionsToQuizDTO()
            {
                QuizId = quizResult.QuizId,
                QuestionIds = new List<int>() { mcqResult.Id }
            };


            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await quizServices.AddQuestionsToQuizAsync(addQuestionsToQuizDTO, 765)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
            await questionServices.DeleteQuestionByID(mcqResult.Id, LoggedInUser);
            await quizServices.DeleteQuizByIDAsync(quizResult.QuizId, LoggedInUser);
        }
    }
}
