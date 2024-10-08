﻿using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Shared;
using SapirProductionFloorManagment.Server.Authentication;
using SapirProductionFloorManagment.Shared.Authentication___Autherization;
using Microsoft.AspNetCore.Authorization;
using NPOI.SS.Formula.Functions;

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

        
        [AllowAnonymous]
        [HttpPost]
        public UserSession GetLoginRequset(User user) 
        {

            try
            {
                using var dbCon = new MainDbContext();
                var userFromDb = dbCon.Users.Where(u => u.UserName  == user.UserName && u.Password == user.Password).First();
                if (userFromDb != null)
                {
                    var jwtAuthenticationManager = new JwtAuthenticationManager(userFromDb);
                    var userSession = jwtAuthenticationManager.GenerateJwtToken(user.UserName, user.Password);
                    if (userSession == null)
                        Unauthorized();
                    else
                        _logger.LogInformation("Success to login at {DateTime.Now}, ID: {userFromDb.UserId}", DateTime.Now, userFromDb.UserId);
                         return userSession;
                }
                else
                    Unauthorized();

            }
            catch (Exception ex) 
            {
                // Log error
                _logger.LogError("GetLoginRequest: {ex.Message}", ex.Message);

            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public UserSession StayLoggedIn(UserSession session)
        {
            using var dbcon = new MainDbContext();
            var user = dbcon.Users.Where(e => e.UserName == session.UserName).FirstOrDefault(); 

            if(user != null)
            {
                return session;
            }
            Unauthorized();
            return null;

        }  



        [HttpGet]
        public string CreateDefaultUser()
        {

            using var dbcon = new MainDbContext();
            try
            {
                var user = dbcon.Users.First();

            }
            catch (Exception ex)
            {
                _logger.LogError("CreateDefaultUser: {ex.Message}", ex.Message);

                if(ex.Message == "Sequence contains no elements")
                {
                    dbcon.CreateDefaultDataForAppFunctionallity();
                    return string.Empty;

                }
               
                return ex.Message.ToString();       

            }
            return string.Empty;

        }
    }

   
}
