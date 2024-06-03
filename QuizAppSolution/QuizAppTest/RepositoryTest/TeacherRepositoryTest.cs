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
    public class TeacherRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        int LoggedInUser;
        IRepository<int, Teacher> teacherRepo;

        [SetUp]
        public async Task setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                        .UseInMemoryDatabase("TeacherRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            teacherRepo = new TeacherRepository(quizAppContext);
        }

        [Test]
        public async Task AddTeacherSuccessTest()
        {
            Teacher user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            //action
            var result = await teacherRepo.Add(user);

            //Assert
            Assert.NotNull(result);
            await teacherRepo.Delete(result.Id);
        }

        [Test]
        public async Task DeleteTeacherSuccessTest()
        {
            Teacher user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await teacherRepo.Add(user);

            //action
            var delResult = await teacherRepo.Delete(result.Id);

            //Assert
            Assert.NotNull(delResult);
            Assert.AreEqual(result.Id, delResult.Id);
        }

        [Test]
        public async Task DeleteTeacherExceptionTest()
        {
            Teacher user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await teacherRepo.Add(user);


            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await teacherRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
            await teacherRepo.Delete(result.Id);

        }

        [Test]
        public async Task UpdateTeacherSuccessTest()
        {
            Teacher user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await teacherRepo.Add(user);
            result.MobileNumber = "9999988776";

            //action
            var updateResult = await teacherRepo.Update(result);

            //Assert
            Assert.NotNull(updateResult);
            Assert.AreEqual(result.Id, updateResult.Id);
            await teacherRepo.Delete(result.Id);

        }

        [Test]
        public async Task UpdateTeacherExceptionTest()
        {
            Teacher user = new Teacher()
            {
                Id = 999,
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await teacherRepo.Update(user)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetTeacherSuccessTest()
        {
            Teacher user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await teacherRepo.Add(user);

            //action
            var getResult = await teacherRepo.Get(result.Id);

            //Assert
            Assert.NotNull(getResult);
            Assert.AreEqual(result.Id, getResult.Id);
            await teacherRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetTeacherExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await teacherRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetAllTeacherSuccessTest()
        {
            Teacher user = new Teacher()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                Designation = "HOD",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };
            var result = await teacherRepo.Add(user);

            //action
            var getResult = await teacherRepo.Get();

            //Assert
            Assert.NotNull(getResult);
            Assert.NotZero(getResult.Count());
            await teacherRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetAllTeacherFailureTest()
        {
            // Action
            var result = await teacherRepo.Get();

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
