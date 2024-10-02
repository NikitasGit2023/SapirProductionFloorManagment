using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductionReportController
    {
        private readonly ILogger<ProductionReportController> _logger;    
        public ProductionReportController(ILogger<ProductionReportController> logger) 
        {
            _logger = logger;
        } 

        [HttpPost]
        public string PostWorkOrderQuantity(WorkOrder wo) 
        {
            try
            {
                using var dbcon = new MainDbContext();
                var workOrder = dbcon.WorkOrdersFromXL.Where(x => x.WorkOrderSN == wo.WorkOrderSN).FirstOrDefault();

                    
                    workOrder.ProducedQuantity =+ wo.ProducedQuantity;
                    workOrder.QuantityLeft = workOrder.QuantityInKg - workOrder.ProducedQuantity;   

                    if(workOrder.ProducedQuantity > workOrder.QuantityInKg) 
                    {
                        workOrder.ProducedQuantity = workOrder.QuantityInKg;
                        workOrder.QuantityLeft = 0;
                    }
                    dbcon.Update(workOrder);
                    dbcon.SaveChanges();
                    return "המידע עודכן בהצלחה";
            }
            catch (Exception ex) 
            {
                _logger.LogError("PostWorkOrderQuantity: {DateTime.Now} , {ex.Message}",DateTime.Now, ex.Message);
                return ex.Message;
            }    
        }

        [HttpGet]
        public List<WorkOrder> GetExistedWorkOrders()
        {
            try
            {
                using var dbcon = new MainDbContext();  
                var workOrders = dbcon.WorkOrdersFromXL.ToList();
                return workOrders;  

            }catch(Exception ex) 
            {
                _logger.LogError("GetExistedWorkOrders: {DateTime.Now} , {ex.Message}", DateTime.Now, ex.Message);
            }
            return new List<WorkOrder>();   
        }    
       
    }
}
