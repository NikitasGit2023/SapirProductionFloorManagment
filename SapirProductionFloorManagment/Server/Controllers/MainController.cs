using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MainController : Controller
    {
        protected ILogger Logger;
        public MainController() 
        {
               
        }
    }
}
