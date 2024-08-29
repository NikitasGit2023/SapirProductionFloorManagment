using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Server.DbContexts;
using SapirProductionFloorManagment.Shared;


namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LoginController : Controller
    {
        [HttpPost]
        public bool GetLoginRequset(User user)
        {
            try
            {
                using var dbContext = new LoginDbContext();
                var existedUser = dbContext.Find(typeof(User), user.UserId);
                var u = existedUser as User;

                if (u != null && u.UserId == user.UserId)
                {
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                //write inside logs file
               

            }
            return false;


        }

    }


}