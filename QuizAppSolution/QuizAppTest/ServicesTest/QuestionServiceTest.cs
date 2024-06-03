using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Models.DTOs;
using QuizApp.Models.DTOs.FillUpsDTOs;
using QuizApp.Models.DTOs.MCQDTOs;
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
    public class QuestionServiceTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IQuestionServices questionServices;
        IQuestionViewServices questionViewServices;
        int LoggedInUser;
        Mock<ILogger<QuestionServices>> mockLogger;
        Mock<ILogger<QuestionViewServices>> QViewMockLogger;
        Mock<ILogger<UserLoginAndRegisterServices>> UserLoginAndRegisterMockLogger;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
            .UseInMemoryDatabase("QuestionServiceTestDB");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices tokenServices = new TokenServices(mockConfig.Object);

            mockLogger = new Mock<ILogger<QuestionServices>>();
            QViewMockLogger = new Mock<ILogger<QuestionViewServices>>();
            UserLoginAndRegisterMockLogger = new Mock<ILogger<UserLoginAndRegisterServices>>();

            IRepository<int, Question> QuestionRepository = new QuestionRepository(quizAppContext);
            IRepository<int, FillUps> fillUpsRepo = new FillUpsRepository(quizAppContext);
            IRepository<int, MultipleChoice> multipleChoiceRepo = new MultipleChoiceRepository(quizAppContext);
            IRepository<int, Teacher> teacherRepo = new TeacherRepository(quizAppContext);
            IRepository<int, Student> studentRepo = new StudentRepository(quizAppContext);
            IRepository<int, User> userRepo = new UserRepository(quizAppContext);
            IRepository<int, UserDetails> userDetailsRepo = new UserDetailRepository(quizAppContext);

            userLoginAndRegisterServices = new UserLoginAndRegisterServices
                                                   (userRepo, userDetailsRepo, 
                                                   tokenServices, teacherRepo, 
                                                   studentRepo, UserLoginAndRegisterMockLogger.Object);
            questionServices = new QuestionServices(QuestionRepository, fillUpsRepo, multipleChoiceRepo, teacherRepo,mockLogger.Object);
            questionViewServices = new QuestionViewServices(QuestionRepository,QViewMockLogger.Object);

            //Action
            try
            {
                UserRegisterInputDTO userRegisterInputDTO = new UserRegisterInputDTO
                {
                    Name = "Ramu",
                    Email = "ramu@gmail.com",
                    MobileNumber = "8977665565",
                    DateOfBirth = new DateTime(2002 / 04 / 13),
                    Password = "ram123",
                    Designation = "UG-Teacher",
                    UserType = "Teacher"
                };
                var result = await userLoginAndRegisterServices.Register(userRegisterInputDTO);
                LoggedInUser = result.Id;
            }
            catch (Exception ex) { }
        }

        [Test]
        public async Task AddMCQQuestionSuccessTest()
        {
            //Arrange
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

            //Action
            var result = await questionServices.AddMCQQuestion(mcq);

            //Assert
            Assert.AreEqual(result.QuestionText, mcq.QuestionText);
            await questionServices.DeleteQuestionByID(result.Id, LoggedInUser);
        }

        [Test]
        public async Task AddMCQQuestionExceptionTest()
        {
            //Arrange
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
                CreatedBy = 999,
                QuestionType = "GK"
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await questionServices.AddMCQQuestion(mcq)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task AddFillUpsSuccessTest()
        {
            //Arrange
            FillUpsDTO fillUps = new FillUpsDTO()
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

            //Action
            var result = await questionServices.AddFillUpsQuestion(fillUps);

            //Assert
            Assert.AreEqual(result.QuestionText, fillUps.QuestionText);
        }

        [Test]
        public async Task AddFillUpsQuestionExceptionTest()
        {
            //Arrange
            FillUpsDTO fillUps = new FillUpsDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Points = 10,
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                CorrectAnswer = "Blue",
                CreatedDate = DateTime.Now,
                CreatedBy = 678,
                QuestionType = "GK"
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await questionServices.AddFillUpsQuestion(fillUps)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {678}", exception.Message);
        }


        [Test]
        public async Task EditFillUpsSuccessTest()
        {
            //Arrange
            FillUpsDTO fillUps = new FillUpsDTO()
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

            var result = await questionServices.AddFillUpsQuestion(fillUps);

            FillUpsUpdateDTO fillUpsUpdateDTO = new FillUpsUpdateDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                CorrectAnswer = "Blue",
                QuestionId = result.Id,
                Points = 17
            };

            //action
            var editResult = await questionServices.EditFillUpsQuestionById(fillUpsUpdateDTO, LoggedInUser);

            //Assert
            Assert.AreEqual(editResult.Points, 17);
        }


        [Test]
        public async Task EditFillUpsExceptionTest1()
        {
            //Arrange
            FillUpsDTO fillUps = new FillUpsDTO()
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

            var result = await questionServices.AddFillUpsQuestion(fillUps);

            FillUpsUpdateDTO fillUpsUpdateDTO = new FillUpsUpdateDTO()
            {
                QuestionId = result.Id,
                Points = 17
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await questionServices.EditFillUpsQuestionById(fillUpsUpdateDTO, 200)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
        }

        [Test]
        public async Task EditFillUpsExceptionTest2()
        {
            //Arrange
            FillUpsDTO fillUps = new FillUpsDTO()
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

            var result = await questionServices.AddFillUpsQuestion(fillUps);

            FillUpsUpdateDTO fillUpsUpdateDTO = new FillUpsUpdateDTO()
            {
                QuestionId = 999,
                Points = 17
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionServices.EditFillUpsQuestionById(fillUpsUpdateDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }


        [Test]
        public async Task EditMCQSuccessTest()
        {
            //Arrange
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

            MCQUpdateDTO mCQUpdateDTO = new MCQUpdateDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Blue",
                QuestionId = result.Id,
                Points = 17
            };

            //action
            var editResult = await questionServices.EditMCQByQuestionID(mCQUpdateDTO, LoggedInUser);

            //Assert
            Assert.AreEqual(editResult.Points, 17);
        }


        [Test]
        public async Task EditMCQExceptionTest1()
        {
            //Arrange
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
                QuestionType = "MCQ"
            };

            var result = await questionServices.AddMCQQuestion(mcq);

            MCQUpdateDTO mCQUpdateDTO = new MCQUpdateDTO()
            {
                QuestionId = result.Id,
                QuestionText = "What is th color of fire ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Orange",
                Points = 17
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await questionServices.EditMCQByQuestionID(mCQUpdateDTO, 200)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
        }


        [Test]
        public async Task EditMCQExceptionTest2()
        {
            //Arrange
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

            MCQUpdateDTO mCQUpdateDTO = new MCQUpdateDTO()
            {
                QuestionId = 287,
                Points = 17
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionServices.EditMCQByQuestionID(mCQUpdateDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {287}", exception.Message);
        }


        [Test]
        public async Task EditQuestionByIdSuccessTest1()
        {
            //Arrange
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


            UpdateQuestionDTO updateQuestionDTO = new UpdateQuestionDTO()
            {
                Id = result.Id,
                QuestionText = "What is th color of Sky ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Blue",
                Points = 17
            };

            //action
            var editResult = await questionServices.EditQuestionByID(updateQuestionDTO, LoggedInUser);

            //Assert
            Assert.AreEqual(editResult.Points, 17);
        }


        [Test]
        public async Task EditQuestionByIdSuccessTest2()
        {
            //Arrange
            FillUpsDTO fillUps = new FillUpsDTO()
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

            var result = await questionServices.AddFillUpsQuestion(fillUps);

            UpdateQuestionDTO updateQuestionDTO = new UpdateQuestionDTO()
            {
                QuestionText = "What is th color of Sky ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                CorrectAnswer = "Blue",
                Id = result.Id,
                Points = 17
            };

            //action
            var editResult = await questionServices.EditQuestionByID(updateQuestionDTO, LoggedInUser);

            //Assert
            Assert.AreEqual(editResult.Points, 17);
        }


        [Test]
        public async Task EditQuestionByIdExceptionTest1()
        {
            //Arrange
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


            UpdateQuestionDTO updateQuestionDTO = new UpdateQuestionDTO()
            {
                Id = 999,
                QuestionText = "What is th color of Sky ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Blue",
                Points = 17
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionServices.EditQuestionByID(updateQuestionDTO, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task EditQuestionByIdExceptionTest2()
        {
            //Arrange
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


            UpdateQuestionDTO updateQuestionDTO = new UpdateQuestionDTO()
            {
                Id = result.Id,
                QuestionText = "What is th color of Sky ?",
                Category = "General",
                DifficultyLevel = DifficultyLevel.Medium,
                Choice1 = "Yellow",
                Choice2 = "Blue",
                Choice3 = "Orange",
                Choice4 = "Red",
                CorrectAnswer = "Blue",
                Points = 17
            };

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await questionServices.EditQuestionByID(updateQuestionDTO, 999)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
        }


        [Test]
        public async Task DeleteQuestionByIDSuccessTest()
        {
            //Arrange
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

            //Action
            var deleteResult = await questionServices.DeleteQuestionByID(result.Id, LoggedInUser);

            //assert
            Assert.AreEqual(deleteResult.Id, result.Id);

        }

        [Test]
        public async Task DeleteQuestionByIDExceptionTest1()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionServices.DeleteQuestionByID(999, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task DeleteQuestionByIDExceptionTest2()
        {
            //Arrange
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
            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToDeleteException>(async () =>
                await questionServices.DeleteQuestionByID(result.Id, 999)
            );

            // Assert
            Assert.AreEqual($"You cant delete this. Only Creator can Delete", exception.Message);
        }


        [Test]
        public async Task SoftDeleteQuestionByIDSuccessTest()
        {
            //Arrange
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

            //Action
            var deleteResult = await questionServices.SoftDeleteQuestionByIDAsync(result.Id, LoggedInUser);

            //assert
            Assert.AreEqual(deleteResult.Id, result.Id);

        }

        [Test]
        public async Task softDeleteQuestionByIDExceptionTest1()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionServices.SoftDeleteQuestionByIDAsync(999, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task softDeleteQuestionByIDExceptionTest2()
        {
            //Arrange
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
            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToDeleteException>(async () =>
                await questionServices.SoftDeleteQuestionByIDAsync(result.Id, 999)
            );

            // Assert
            Assert.AreEqual($"You cant delete this. Only Creator can Delete", exception.Message);
        }

        [Test]
        public async Task UndoSoftDeleteQuestionByIDSuccessTest()
        {
            //Arrange
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
            var softdeleteResult = await questionServices.SoftDeleteQuestionByIDAsync(result.Id, LoggedInUser);

            //Action
            var deleteResult = await questionServices.UndoSoftDeleteQuestionByIDAsync(result.Id, LoggedInUser);

            //assert
            Assert.AreEqual(deleteResult.Id, result.Id);

        }

        [Test]
        public async Task UndosoftDeleteQuestionByIDExceptionTest1()
        {
            //arrange
            //Arrange
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
            var softdeleteResult = await questionServices.SoftDeleteQuestionByIDAsync(result.Id, LoggedInUser);

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuestionException>(async () =>
                await questionServices.UndoSoftDeleteQuestionByIDAsync(999, LoggedInUser)
            );

            // Assert
            Assert.AreEqual($"No Question found for the QuestionId {999}", exception.Message);
        }

        [Test]
        public async Task UndosoftDeleteQuestionByIDExceptionTest2()
        {
            //Arrange
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
            var softdeleteResult = await questionServices.SoftDeleteQuestionByIDAsync(result.Id, LoggedInUser);

            // Action
            var exception = Assert.ThrowsAsync<UnauthorizedToEditException>(async () =>
                await questionServices.UndoSoftDeleteQuestionByIDAsync(result.Id, 999)
            );

            // Assert
            Assert.AreEqual($"You cant edit this. Only Creator can Edit", exception.Message);
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
        }
    }
}
