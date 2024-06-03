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
    public class UserDetailsRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        IRepository<int, UserDetails> userDetailRepo;
        IRepository<int, User> userRepo;
        int userID;

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                            .UseInMemoryDatabase("UserDetailsRepositoryTestDB");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            userRepo = new UserRepository(quizAppContext);

            userDetailRepo = new UserDetailRepository(quizAppContext);

            
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
        public async Task AddUserDetailsSuccessTest()
        {
            //Arrange
            var userDetails = new UserDetails
            {
                UserId = userID,
                Password = new byte[] { 0x20, 0x21 },
                PasswordHashKey = new byte[] { 0x22, 0x23 }
            };

            //action
            var addUserDetailsResult = await userDetailRepo.Add(userDetails);

            //Assert
            Assert.NotNull(addUserDetailsResult);
            Assert.AreEqual(addUserDetailsResult.UserId, userDetails.UserId);
            await userDetailRepo.Delete(userID);
        }

        [Test]
        public async Task DeleteUserDetailsSuccessTest()
        {
            //Arrange
            var userDetails = new UserDetails
            {
                UserId = userID,
                Password = new byte[] { 0x20, 0x21 },
                PasswordHashKey = new byte[] { 0x22, 0x23 }
            };
            var addUserDetailsResult = await userDetailRepo.Add(userDetails);

            //action
            var delResult = await userDetailRepo.Delete(userID);

            //Assert
            Assert.NotNull(delResult);
            Assert.AreEqual(delResult.UserId, userDetails.UserId);
        }

        [Test]
        public async Task DeleteUserDetailsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userDetailRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task UpdateUserDetailsExceptionTest()
        {
            //Arrange
            var userDetails = new UserDetails
            {
                UserId = 999,
                Password = new byte[] { 0x20, 0x21 },
                PasswordHashKey = new byte[] { 0x22, 0x23 }
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userDetailRepo.Update(userDetails)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task UpdateUserDetailsSuccessTest()
        {
            //Arrange
            var userDetails = new UserDetails
            {
                UserId = userID,
                Password = new byte[] { 0x20, 0x21 },
                PasswordHashKey = new byte[] { 0x22, 0x23 }
            };
            var addUserDetailsResult = await userDetailRepo.Add(userDetails);

            //action
            var updateResult = await userDetailRepo.Update(addUserDetailsResult);

            //Assert
            Assert.NotNull(updateResult);
            Assert.AreEqual(updateResult.UserId, userDetails.UserId);
        }

        [Test]
        public async Task GetUserDetailsSuccessTest()
        {
            //Arrange
            var userDetails = new UserDetails
            {
                UserId = userID,
                Password = new byte[] { 0x20, 0x21 },
                PasswordHashKey = new byte[] { 0x22, 0x23 }
            };

            var addUserDetailsResult = await userDetailRepo.Add(userDetails);

            //action
            var result = await userDetailRepo.Get(addUserDetailsResult.UserId);

            //Assert
            Assert.NotNull(result);
            Assert.AreEqual(result.UserId, addUserDetailsResult.UserId);
            await userDetailRepo.Delete(userID);
        }

        [Test]
        public async Task GetUserDetailsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userDetailRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetAllUserDetailsSuccessTest()
        {
            //Arrange
            var userDetails = new UserDetails
            {
                UserId = userID,
                Password = new byte[] { 0x20, 0x21 },
                PasswordHashKey = new byte[] { 0x22, 0x23 }
            };

            var addUserDetailsResult = await userDetailRepo.Add(userDetails);

            //action
            var result = await userDetailRepo.Get();

            //Assert
            Assert.NotNull(result);
            Assert.NotZero(result.Count());
            await userDetailRepo.Delete(userID);
        }

        [Test]
        public async Task GetAllUserDetailsExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userDetailRepo.Get()
            );

            // Assert
            Assert.AreEqual($"No such user found", exception.Message);
        }
    }
}
