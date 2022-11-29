using IdentityModel.OidcClient;
using k8s.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserAPI.Controllers;
using UserAPI.Data;
using UserAPI.Models;
using UserAPI.Services.Interfaces;

namespace UserAPI.Test
{
    public class UserModelTest
    {

        private DbContextClass _dbContext;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Get_User_List_Return_OK()
        {
            //Arrange
            var mockService = new Mock<IUserService>();
            mockService
                .Setup(m => m.GetUserList())
                .Returns(new List<User>() { new User() { UserId = 1, UserName= "Hieu", Address = "1329/4"} });
            var _controller = new UserController(mockService.Object);

            //ACT
            var result = _controller.UserList();

            //ASSERT
            Assert.IsInstanceOf<List<User>>(result);
        }
    }
}