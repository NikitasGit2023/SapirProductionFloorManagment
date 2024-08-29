using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProductionReportController : Controller
    {
        [HttpPost]
        public string PostWorkOrderQuantity(WorkOrder wo) 
        {
            try
            {

                using var dbcon = new MainDbContext();
                var workOrder = dbcon.WorkOrders.Where(x => x.Id == wo.Id).FirstOrDefault();
                if (workOrder != null)
                {
                    if (workOrder.Quantity > wo.Quantity)
                        return "הכמות שהזנת קטנה מהכמות הקיימת במערכת";
                    workOrder.Quantity = wo.Quantity;       
                    dbcon.Update(workOrder);
                    dbcon.SaveChanges();
                    return "המידע עודכן בהצלחה";
                }
                return "פקודת העבודה לא נמצאה במערכת";

            }
            catch (Exception ex) 
            {
                return ex.Message;
            }   
            
        }

        [HttpGet]
        public List<WorkOrder> GetExistedWorkOrders()
        {
            try
            {
                using var dbcon = new MainDbContext();  
                var workOrders = dbcon.WorkOrders.ToList();
                dbcon.SaveChanges();
                return workOrders;  

            }catch(Exception ex) 
            {
            }
            return new List<WorkOrder>();   
            
        }    

       
    }
}
