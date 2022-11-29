using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Generators;
using RabbitMQ.Client;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UserAPI.Authorization;
using UserAPI.Data;
using UserAPI.Filter;
using UserAPI.Helpers;
using UserAPI.Models;
using UserAPI.Models.Authenticate;
using UserAPI.Services.Interfaces;
using UserAPI.Utility;

namespace UserAPI.Services
{
    //[ExcludeFromCodeCoverage]
    public class UserService : IUserService
    {
        private readonly DbContextClass _dbContext;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public UserService(DbContextClass dbContext,
                           IJwtUtils jwtUtils,
                           IOptions<AppSettings> appSettings)
        {
            _dbContext = dbContext;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }

        public User AddUser(User userModel)
        {
            var result = _dbContext.Users.Add(userModel);
            _dbContext.SaveChanges();

            //send email to rabbtiMQ
            var RabbitMQServer = "";
            var RabbitMQUserName = "";
            var RabbutMQPassword = "";

            RabbitMQServer = StaticConfigurationManager.AppSetting["RabbitMQ:RabbitURL"];
            RabbitMQUserName = StaticConfigurationManager.AppSetting["RabbitMQ:Username"];
            RabbutMQPassword = StaticConfigurationManager.AppSetting["RabbitMQ:Password"];
            try
            {
                var factory = new ConnectionFactory()
                { HostName = RabbitMQServer, UserName = RabbitMQUserName, Password = RabbutMQPassword };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    //Direct Exchange Details like name and type of exchange
                    channel.ExchangeDeclare(StaticConfigurationManager.AppSetting["RabbitMqSettings:ExchangeName"], StaticConfigurationManager.AppSetting["RabbitMqSettings:ExchhangeType"]);

                    //Declare Queue with Name and a few property related to Queue like durabality of msg, auto delete and many more
                    channel.QueueDeclare(queue: StaticConfigurationManager.AppSetting["RabbitMqSettings:QueueName"],
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    //Bind Queue with Exhange and routing details
                    channel.QueueBind(queue: StaticConfigurationManager.AppSetting["RabbitMqSettings:QueueName"], exchange: StaticConfigurationManager.AppSetting["RabbitMqSettings:ExchangeName"], routingKey: StaticConfigurationManager.AppSetting["RabbitMqSettings:RouteKey"]);

                    //Seriliaze object using Newtonsoft library
                    string productDetail = JsonConvert.SerializeObject(userModel);
                    var body = Encoding.UTF8.GetBytes(productDetail);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    //publish msg 
                    channel.BasicPublish(exchange: StaticConfigurationManager.AppSetting["RabbitMqSettings:ExchangeName"],
                                         routingKey: StaticConfigurationManager.AppSetting["RabbitMqSettings:RouteKey"],
                                         basicProperties: properties,
                                         body: body);
                }
            }

            catch (Exception)
            {
            }



            return result.Entity;
        }

        public bool DeleteUser(int Id)
        {
            var filteredData = _dbContext.Users.Where(x => x.UserId == Id).FirstOrDefault();
            var result = _dbContext.Remove(filteredData);
            _dbContext.SaveChanges();
            return result != null ? true : false;
        }

        public User GetUserById(int id)
        {
            return _dbContext.Users.Where(x => x.UserId == id).FirstOrDefault();
        }

        public IEnumerable<User> GetUserList()
        {
            return _dbContext.Users.ToList();
        }

        public User UpdateUser(User product)
        {
            var result = _dbContext.Users.Update(product);
            _dbContext.SaveChanges();
            return result.Entity;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _dbContext.Users.SingleOrDefault(x => x.UserName == model.Username);

            // validate
            //if (user == null || !BCrypt.Verify(model.Password, user.PasswordHash))
            //    throw new AppException("Username or password is incorrect");

            // authentication successful so generate jwt token
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken);
        }
    }
}
