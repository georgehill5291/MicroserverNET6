using UserAPI.Models;
using UserAPI.Models.Authenticate;

namespace UserAPI.Services.Interfaces
{
    public interface IUserService
    {
        public IEnumerable<User> GetUserList();
        public User GetUserById(int id);
        public User AddUser(User product);
        public User UpdateUser(User product);
        public bool DeleteUser(int Id);
        public AuthenticateResponse Authenticate(AuthenticateRequest model);
    }
}
