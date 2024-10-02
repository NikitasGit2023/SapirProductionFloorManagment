using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ViewInformationController
    {
        private readonly ILogger<ViewInformationController> _logger;
        public ViewInformationController(ILogger<ViewInformationController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Product> GetExistedProducts()
        {
            try
            {
                using var dbcon = new MainDbContext();
                var products = dbcon.Products.ToList();
                return products;            

            }
            catch (Exception ex)
            {
                _logger.LogError("GetExistedWorkOrders: {DateTime.Now} , {ex.Message}", DateTime.Now, ex.Message);
            }
            return new List<Product>();
        }


        [HttpGet]
        public List<Customer> GetExistedCustomers()
        {
            try
            {
                using var dbcon = new MainDbContext();
                var customers = dbcon.Customers.ToList();
                return customers;

            }
            catch (Exception ex)
            {
                _logger.LogError("GetExistedWorkOrders: {DateTime.Now} , {ex.Message}", DateTime.Now, ex.Message);
            }
            return new List<Customer>();
        }

    }
}
