using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Shared;
using SapirProductionFloorManagment.Server.Authentication;
using SapirProductionFloorManagment.Shared.Authentication___Autherization;
using Microsoft.AspNetCore.Authorization;

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
                var userFromDb = dbCon.Users.Where(u => u.FullName  == user.FullName && u.Password == user.Password).First();
                if (userFromDb != null)
                {
                    var jwtAuthenticationManager = new JwtAuthenticationManager(userFromDb);
                    var userSession = jwtAuthenticationManager.GenerateJwtToken(user.FullName, user.Password);
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



        [HttpPost]
        public string PostWorkOrderQuantity(WorkOrdersTableContext wo)
        {
            try
            {
                using var dbcon = new MainDbContext();
                var workOrder = dbcon.WorkOrdersFromXL.Where(x => x.WorkOrderSN == wo.WorkOrderSN).FirstOrDefault();

                if (workOrder.Quantity > wo.Quantity)
                    return "הכמות שהזנת קטנה מהכמות הקיימת במערכת";
                workOrder.Quantity = wo.Quantity;
                dbcon.Update(workOrder);
                dbcon.SaveChanges();
                return "המידע עודכן בהצלחה";
            }
            catch (Exception ex)
            {
                _logger.LogError("PostWorkOrderQuantity: {DateTime.Now} , {ex.Message}", DateTime.Now, ex.Message);
                return ex.Message;
            }
        }

        //optional
        [HttpPost]
        public bool RegisterUser(User user)
        {
            return false;
        }
    }
}
