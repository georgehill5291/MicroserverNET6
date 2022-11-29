using Microsoft.AspNetCore.Mvc;
using UserAPI.Authorization;
using UserAPI.Filter;
using UserAPI.Models;
using UserAPI.Models.Authenticate;
using UserAPI.Services;
using UserAPI.Services.Interfaces;

namespace UserAPI.Controllers
{
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }
        [HttpGet]
        public IEnumerable<User> UserList()
        {
            var userList = _userService.GetUserList();
            return userList;
        }

        [HttpGet("{id}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<User>))]
        public User GetUserById(int id)
        {
            return _userService.GetUserById(id);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public User AddUser(User user)
        {
            return _userService.AddUser(user);
        }

        [HttpPut]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<User>))]
        public User UpdateUser(User user)
        {
            return _userService.UpdateUser(user);
        }


        [HttpDelete("{id}")]
        [ServiceFilter(typeof(ValidateEntityExistsAttribute<User>))]
        public bool DeleteUser(int id)
        {
            return _userService.DeleteUser(id);
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Authenticate(model);
            return Ok(response);
        }

    }
}
