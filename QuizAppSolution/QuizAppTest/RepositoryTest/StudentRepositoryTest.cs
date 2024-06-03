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
    public class StudentRepositoryTest
    {
        QuizAppContext quizAppContext;
        IUserLoginAndRegisterServices userLoginAndRegisterServices;
        int LoggedInUser;
        IRepository<int, Student> studentRepo;

        [SetUp]
        public async Task setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder()
                                                        .UseInMemoryDatabase("StudentRepositoryTest");

            quizAppContext = new QuizAppContext(optionsBuilder.Options);

            studentRepo = new StudentRepository(quizAppContext);
        }

        [Test]
        public async Task AddStudentSuccessTest()
        {
            Student user = new Student()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            //action
            var result = await studentRepo.Add(user);

            //Assert
            Assert.NotNull(result);
            await studentRepo.Delete(result.Id);
        }

        [Test]
        public async Task DeleteStudentSSuccessTest()
        {
            Student user = new Student()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            var result = await studentRepo.Add(user);

            //action
            var delResult = await studentRepo.Delete(result.Id);

            //Assert
            Assert.NotNull(delResult);
            Assert.AreEqual(result.Id, delResult.Id);
        }

        [Test]
        public async Task DeleteStudentSExceptionTest()
        {
            Student user = new Student()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            var result = await studentRepo.Add(user);


            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await studentRepo.Delete(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
            await studentRepo.Delete(result.Id);

        }

        [Test]
        public async Task UpdateStudentSSuccessTest()
        {
            Student user = new Student()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            var result = await studentRepo.Add(user);
            result.MobileNumber = "9999988776";

            //action
            var updateResult = await studentRepo.Update(result);

            //Assert
            Assert.NotNull(updateResult);
            Assert.AreEqual(result.Id, updateResult.Id);
            await studentRepo.Delete(result.Id);

        }

        [Test]
        public async Task UpdateStudentSExceptionTest()
        {
            Student user = new Student()
            {
                Id= 999,
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };


            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await studentRepo.Update(user)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetStudentSSuccessTest()
        {
            Student user = new Student()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            var result = await studentRepo.Add(user);

            //action
            var getResult = await studentRepo.Get(result.Id);

            //Assert
            Assert.NotNull(getResult);
            Assert.AreEqual(result.Id, getResult.Id);
            await studentRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetStudentSExceptionTest()
        {
            // Action
            var exception = Assert.ThrowsAsync<NoSuchUserException>(async () =>
                await studentRepo.Get(999)
            );

            // Assert
            Assert.AreEqual($"No user with the given ID : {999}", exception.Message);
        }

        [Test]
        public async Task GetAllStudentSSuccessTest()
        {
            Student user = new Student()
            {
                Name = "Ramu",
                Email = "ram@gmail.com",
                MobileNumber = "9677381857",
                EducationQualification = "B.tech - IT",
                DateOfBirth = new DateTime(2004 / 02 / 20)
            };

            var result = await studentRepo.Add(user);

            //action
            var getResult = await studentRepo.Get();

            //Assert
            Assert.NotNull(getResult);
            Assert.NotZero(getResult.Count());
            await studentRepo.Delete(result.Id);
        }

        [Test]
        public async Task GetAllStudentsFailureTest()
        {
            // Action
            var result = await studentRepo.Get();

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
