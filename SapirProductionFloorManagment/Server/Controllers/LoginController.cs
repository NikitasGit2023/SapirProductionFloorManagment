using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.Controllers
{

    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : Controller
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }


        [HttpPost]
        public HttpResponseMessage GetLoginRequest(User user)
        {
            try
            {
                using var dbContext = new MainDbContext();
                var existedUser = dbContext.Users.Find(user.UserId);

                if (existedUser != null)
                {
                    // Serialize user object to JSON
                    var json = JsonSerializer.Serialize(user.UserId);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };

                    // Log success
                    _logger.LogInformation("Success to login at {DateTime.Now}, ID: {user.UserId}", DateTime.Now, user.UserId);
                    return responseMessage;
                }
                else
                {
                    // User not found
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("User not found.")
                    };

                    _logger.LogWarning("Failed to login at {DateTime.Now}, ID: {user.UserId}", DateTime.Now, user.UserId);
                    return responseMessage;
                }
            }
            catch (Exception ex)
            {
                // Log error
                _logger.LogError("GetLoginRequest: {ex.Message}", ex.Message);

                // Return internal server error
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("An error occurred.")
                };

                return responseMessage;
            }
        }

        public bool GetLogOutRequset()
        {
            return false;
        }

        [HttpPost]
        public bool RegisterUser(User user)
        {
            return false;
        }
    }
}
