using Microsoft.EntityFrameworkCore;
using QuizApp.Contexts;
using QuizApp.Exceptions;
using QuizApp.Interfaces;
using QuizApp.Models;
using QuizApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizAppTest.RepositoryTest
{
    public class QuizRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IRepository<int, UserDetails> userDetailRepo;
        IRepository<int, User> userRepo;
        IRepository<int, Question> questionRepo;
        IRepository<int, Quiz> quizRepo;

        int userID;
        int questionId;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                            .UseInMemoryDatabase("QuizRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            userRepo = new UserRepository(quizAppContext);
            quizRepo = new QuizRepository(quizAppContext);

            userDetailRepo = new UserDetailRepository(quizAppContext);
            await seedData();
        }

        private async Task seedData()
        {
            if((await userRepo.Get()).Count() == 0)
            {
                User user = new Teacher
                {
                    Name = "Test User",
                    Email = "test@example.com",
                    MobileNumber = "1234567890",
                    DateOfBirth = new System.DateTime(1990, 1, 1),
                    Designation = "HOD"
                };
                var result = await userRepo.Add(user);
                userID = result.Id;
            }
        }


        [Test]
        public async Task AddQuizSuccessTest()
        {
            //Arrange
            Quiz quiz = new Quiz()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = userID,
                QuizDescription = "Sample Test",
                IsMultipleAttemptAllowed = true,
                NumOfQuestions = 1,
                QuizQuestions = null
            };

            //action
            var quizResult = await quizRepo.Add(quiz);

            //Assert 
            Assert.NotNull(quizResult);
            await quizRepo.Delete(quizResult.Id);
        }

        [Test]
        public async Task DeleteQuizSuccessTest()
        {
            //Arrange
            Quiz quiz = new Quiz()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = userID,
                QuizDescription = "Sample Test",
                IsMultipleAttemptAllowed = true,
                NumOfQuestions = 1,
                QuizQuestions = null
            };
            var quizResult = await quizRepo.Add(quiz);

            //Action
            var delResult = await quizRepo.Delete(quizResult.Id);

            //Assert 
            Assert.NotNull(delResult);
            Assert.AreEqual(delResult.Id, quizResult.Id);
        }

        [Test]
        public async Task DeleteQuizExceptionTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task UpdateQuizSuccessTest()
        {
            //Arrange
            Quiz quiz = new Quiz()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = userID,
                QuizDescription = "Sample Test",
                IsMultipleAttemptAllowed = true,
                NumOfQuestions = 1,
                QuizQuestions = null
            };
            var quizResult = await quizRepo.Add(quiz);
            quizResult.QuizType = "Sample Testing";

            //Action
            var updateResult = await quizRepo.Update(quizResult);

            //Assert 
            Assert.NotNull(updateResult);
            Assert.AreEqual(updateResult.QuizType, quizResult.QuizType);
            await quizRepo.Delete(updateResult.Id);
        }

        [Test]
        public async Task UpdateQuizExceptionTest()
        {
            //Arrange
            Quiz quiz = new Quiz()
            {
                Id = 999,
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = userID,
                QuizDescription = "Sample Test",
                IsMultipleAttemptAllowed = true,
                NumOfQuestions = 1,
                QuizQuestions = null
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizRepo.Update(quiz)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task GetQuizSuccessTest()
        {
            //Arrange
            Quiz quiz = new Quiz()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = userID,
                QuizDescription = "Sample Test",
                IsMultipleAttemptAllowed = true,
                NumOfQuestions = 1,
                QuizQuestions = null
            };
            var quizResult = await quizRepo.Add(quiz);

            //Action
            var getResult = await quizRepo.Get(quizResult.Id);

            //Assert 
            Assert.NotNull(getResult);
            Assert.AreEqual(getResult.Id,quizResult.Id);
            await quizRepo.Delete(getResult.Id);
        }

        [Test]
        public async Task GetQuizExceptionTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"Quiz with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task GetAllQuizSuccessTest()
        {
            //Arrange
            Quiz quiz = new Quiz()
            {
                QuizName = "Test",
                QuizType = "Testing",
                QuizCreatedBy = userID,
                QuizDescription = "Sample Test",
                IsMultipleAttemptAllowed = true,
                NumOfQuestions = 1,
                QuizQuestions = null
            };
            var quizResult = await quizRepo.Add(quiz);

            //Action
            var getResult = await quizRepo.Get();

            //Assert 
            Assert.NotZero(getResult.Count());
            await quizRepo.Delete(quizResult.Id);
        }

        [Test]
        public async Task GetAllQuizExceptionTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoSuchQuizException>(async () =>
                await quizRepo.Get()
            );

            // Assert
            Assert.AreEqual($"Quiz Not Found", exception.Message);
        }

    }
}
