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
    public class ResponseRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IRepository<int, UserDetails> userDetailRepo;
        IRepository<int, User> userRepo;
        IRepository<int, Question> questionRepo;
        IRepository<int, Quiz> quizRepo;
        IRepository<int, Response> responseRepo;

        int userID;
        int quizId;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                            .UseInMemoryDatabase("QuizRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            userRepo = new UserRepository(quizAppContext);
            quizRepo = new QuizRepository(quizAppContext);
            responseRepo = new ResponseRepository(quizAppContext);

            userDetailRepo = new UserDetailRepository(quizAppContext);
            await seedData();
        }

        private async Task seedData()
        {
            if ((await userRepo.Get()).Count() == 0)
            {
                User user = new Student
                {
                    Name = "Test User",
                    Email = "test@example.com",
                    MobileNumber = "1234567890",
                    DateOfBirth = new System.DateTime(1990, 1, 1),
                    EducationQualification = "B.Tech - AI and DS"
                };
                var result = await userRepo.Add(user);
                userID = result.Id;
            }

            try
            {
                await quizRepo.Get();
            }
            catch (Exception ex) 
            {
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
                quizId = quizResult.Id;
            }
        }


        [Test]
        public async Task AddResponseSuccessTest()
        {
            //Arrange
            Response response = new Response()
            {
                QuizId = quizId,
                ScoredPoints = 10,
                StartTime = DateTime.Now,
                EndTime = null,
                UserId = userID
            };

            //Action
            var result = await responseRepo.Add(response);

            //assert
            Assert.NotNull(result);
            await responseRepo.Delete(result.Id);
        }

        [Test]
        public async Task DeleteResponseSuccessTest()
        {
            //Arrange
            Response response = new Response()
            {
                QuizId = quizId,
                ScoredPoints = 10,
                StartTime = DateTime.Now,
                EndTime = null,
                UserId = userID
            };
            var result = await responseRepo.Add(response);

            //Action
            var delResult = await responseRepo.Delete(result.Id);

            //assert
            Assert.NotNull(delResult);
            Assert.AreEqual(delResult.Id, result.Id);
        }

        [Test]
        public async Task DeleteResponseExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchResponseException>(async () =>
                await responseRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"Response with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task UpdateResponseSuccessTest()
        {
            //Arrange
            Response response = new Response()
            {
                QuizId = quizId,
                ScoredPoints = 10,
                StartTime = DateTime.Now,
                EndTime = null,
                UserId = userID
            };
            var result = await responseRepo.Add(response);
            result.EndTime = DateTime.Now;

            //Action
            var UpdateResult = await responseRepo.Update(result);

            //assert
            Assert.NotNull(UpdateResult);
            Assert.AreEqual(UpdateResult.Id, result.Id);
            await responseRepo.Delete(result.Id);
        }

        [Test]
        public async Task UpdateResponseExceptionTest()
        {
            //Arrange
            Response response = new Response()
            {
                Id = 999,
                QuizId = quizId,
                ScoredPoints = 10,
                StartTime = DateTime.Now,
                EndTime = null,
                UserId = userID
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchResponseException>(async () =>
                await responseRepo.Update(response)
            );

            // Assert
            Assert.AreEqual($"Response with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task GetResponseSuccessTest()
        {
            //Arrange
            Response response = new Response()
            {
                QuizId = quizId,
                ScoredPoints = 10,
                StartTime = DateTime.Now,
                EndTime = null,
                UserId = userID
            };
            var result = await responseRepo.Add(response);

            //Action
            var getResult = await responseRepo.Get(result.Id);

            //assert
            Assert.NotNull(getResult);
            Assert.AreEqual(getResult.Id, result.Id);
            await responseRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetResponseExceptionTest()
        {

            // Action
            var exception = Assert.ThrowsAsync<NoSuchResponseException>(async () =>
                await responseRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"Response with the QuizId : {999} not found", exception.Message);
        }

        [Test]
        public async Task GetAllResponseSuccessTest()
        {
            //Arrange
            Response response = new Response()
            {
                QuizId = quizId,
                ScoredPoints = 10,
                StartTime = DateTime.Now,
                EndTime = null,
                UserId = userID
            };
            var result = await responseRepo.Add(response);

            //Action
            var getResult = await responseRepo.Get();

            //assert
            Assert.NotNull(getResult);
            Assert.NotZero(getResult.Count());
            await responseRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetAllResponseFailureTest()
        {

            // Action
            var result = await responseRepo.Get();

            // Assert
            Assert.IsEmpty(result);
        }

    }
}
