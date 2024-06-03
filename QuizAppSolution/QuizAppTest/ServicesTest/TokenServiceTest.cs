using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using QuizApp.Interfaces;
using QuizApp.Services;
using QuizApp.Models;
using QuizApp.Models.DTOs.UserDTOs;

namespace QuizAppTest.ServicesTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TokenServiceTest()
        {
            //Arrange
            Mock<IConfigurationSection> configurationJWTSection = new Mock<IConfigurationSection>();
            configurationJWTSection.Setup(x => x.Value).Returns("This is the dummy key for Quiz App Mini Project given by Genspark training team");
            Mock<IConfigurationSection> configTokenSection = new Mock<IConfigurationSection>();
            configTokenSection.Setup(x => x.GetSection("JWT")).Returns(configurationJWTSection.Object);

            Mock<IConfiguration> mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(x => x.GetSection("TokenKey")).Returns(configTokenSection.Object);
            ITokenServices service = new TokenServices(mockConfig.Object);


            //Action
            var token = service.GenerateToken(new User { Id = 102 });

            Assert.IsNotNull(token);
        }
    }
}