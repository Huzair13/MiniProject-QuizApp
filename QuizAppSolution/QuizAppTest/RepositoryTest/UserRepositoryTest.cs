using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
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
    public class UserRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        int LoggedInUser;
        IRepository<int,User> userRepo;

        [SetUp]
        public async Task setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                        .UseInMemoryDatabase("UserRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            userRepo = new UserRepository(quizAppContext);
        }

        [Test]
        public async Task AddUserSuccessTest()
        {
            User user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            //action
            var result = await userRepo.Add(user);

            //Assert
            Assert.NotNull(result);
            await userRepo.Delete(result.Id);
        }

        [Test]
        public async Task DeleteUserSuccessTest()
        {
            User user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await userRepo.Add(user);

            //action
            var delResult = await userRepo.Delete(result.Id);

            //Assert
            Assert.NotNull(delResult);
            Assert.AreEqual(result.Id, delResult.Id);
        }

        [Test]
        public async Task DeleteUserExceptionTest()
        {
            User user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await userRepo.Add(user);


            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
            await userRepo.Delete(result.Id);

        }

        [Test]
        public async Task UpdateUserSuccessTest()
        {
            User user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await userRepo.Add(user);
            result.MobileNumber = "9999988776";

            //action
            var updateResult = await userRepo.Update(result);

            //Assert
            Assert.NotNull(updateResult);
            Assert.AreEqual(result.Id, updateResult.Id);
            await userRepo.Delete(result.Id);

        }

        [Test]
        public async Task UpdateUserExceptionTest()
        {
            User user = new Teacher()
            {
                Id= 999,
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userRepo.Update(user)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetUserSuccessTest()
        {
            User user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await userRepo.Add(user);

            //action
            var getResult = await userRepo.Get(result.Id);

            //Assert
            Assert.NotNull(getResult);
            Assert.AreEqual(result.Id, getResult.Id);
            await userRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetUserExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await userRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetAllUserSuccessTest()
        {
            User user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await userRepo.Add(user);

            //action
            var getResult = await userRepo.Get();

            //Assert
            Assert.NotNull(getResult);
            Assert.NotZero(getResult.Count());
            await userRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetAllUserFailureTest()
        {
            // Action
            var result = await userRepo.Get();

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
