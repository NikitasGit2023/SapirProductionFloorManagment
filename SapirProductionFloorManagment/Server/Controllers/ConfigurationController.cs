using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : Controller
    {

        [HttpPost]
        public void GetNumberOne(int num)
        {
            Console.WriteLine(num);

        }

        
    }
}
