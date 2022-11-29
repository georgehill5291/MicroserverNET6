using UserAPI.Models.Entities;

namespace UserAPI.Models.Authenticate
{
    public class AuthenticateResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Address { get; set; }
        public Role Role { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(User user, string token)
        {
            Id = user.UserId;
            UserName = user.UserName;
            Address = user.Address;                        
            Role = user.Role;
            Token = token;
        }
    }
}
