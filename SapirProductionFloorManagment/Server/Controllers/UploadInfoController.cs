using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Crypto;
using SapirProductionFloorManagment.Client.Shared.Tables;
using SapirProductionFloorManagment.Server.BackgroundTasks;
using SapirProductionFloorManagment.Shared;
using System.Collections.Immutable;
using System.Linq.Dynamic.Core;
namespace SapirProductionFloorManagment.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class UploadInfoController : Controller  
    {
        private readonly ILogger<UploadInfoController>? _logger;

        public UploadInfoController(ILogger<UploadInfoController> logger) : base()
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<string> UpdateOrder(WorkOrder wo)
        {
            if (wo == null) 
            {
              return await Task.FromResult("");
            }
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Update(wo);
                dbcon.SaveChanges();
            }
            catch
            (Exception ex)
            {
                _logger?.LogError("UpdateOrder: {ex.Message}", ex.Message);
                return await Task.FromResult(ex.Message);   
            }
            return await Task.FromResult("המידע נמחק בהצלחה");

        }

        [HttpPost]
        public async Task<string> RemoveOrder(WorkOrder wo)
        {
            if (wo == null)
            {
                return await Task.FromResult("");
            }
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.WorkOrdersFromXL.Remove(wo);
                dbcon.SaveChanges();

                var workPlansToRemove = dbcon.ActiveWorkPlans.Where(e => e.WorkOrderSN == wo.WorkOrderSN).ToList();
                foreach(var workPlan in workPlansToRemove) 
                {
                    dbcon.ActiveWorkPlans.Remove(workPlan);
                    dbcon.SaveChanges();
                }
                var productionTimeCalculator = new ProductionTimeScheduler(_logger);
                productionTimeCalculator.RegenerateWorkPlans(wo);

            }
            catch
            (Exception ex)
            {
                _logger?.LogError("DeleteOrder: {ex.Message}", ex.Message);
                return await Task.FromResult(ex.Message);
            }
            return await Task.FromResult("המידע עודכן בהצלחה");

        }

        [HttpPost]
        public async Task<string> AddOrder(WorkOrder wo)
        {
            if (wo == null)
            {
                return await Task.FromResult("");
            }
            try
            {
                using var dbcon = new MainDbContext();
                var workPlan = dbcon.WorkOrdersFromXL.Where(e => e.WorkOrderSN == wo.WorkOrderSN).FirstOrDefault(); 
                if(workPlan == null)
                {
                    dbcon.Add(wo);
                    dbcon.SaveChanges();

                }
                else
                {
                    return "לא ניתן להוסיף פקודת עבודה עם אותו מספר סריאלי";
                }
                await RecreateOrdersTable(wo);

            }
            catch
            (Exception ex)
            {
                _logger?.LogError("AddOrder: {ex.Message}", ex.Message);
                return await Task.FromResult(ex.Message);
            }
            return await Task.FromResult("המידע עודכן בהצלחה");

        }


        [HttpPost]
        public void PostWorkOrdersTable(List<WorkOrder> table)
        {
            try
            {
                using var dbcon = new MainDbContext();
                for (var i = 0; i < table.Count; i++)
                {
                    var row = table[i];
                    var rowFromDb = dbcon.WorkOrdersFromXL.FirstOrDefault(e => e.WorkOrderSN == row.WorkOrderSN);       

                    if (rowFromDb != null)
                    {
                        continue;
                    }
                    dbcon.WorkOrdersFromXL.Add(row);
                    dbcon.SaveChanges();

                }
                _logger?.LogInformation("Work orders table filled with new data from XL file");
            }
            catch (Exception ex)
            {
                _logger?.LogError("PostWorkOrdersTable: {ex.Message}", ex.Message );
            }

            }

        [HttpGet]
        public async Task<List<WorkOrder>> GetExistedWorkOrders()
        {
            try
            {
                using var dbcon = new MainDbContext();
                var workOrders = dbcon.WorkOrdersFromXL.ToList();
                return await Task.FromResult(workOrders);



            }
            catch (Exception ex)
            {
                _logger?.LogError("GetExistedWorkOrders: {ex.Message}", ex.Message);

            }
            return new List<WorkOrder>();

        }


        [HttpGet]
        public async Task<List<string>> GetLinesName()
        {
            try
            {
                using var dbcon = new MainDbContext();
                var linesName = dbcon.Lines.Select(e => e.Name).Distinct().ToList();
                return await Task.FromResult(linesName);

            }
            catch (Exception ex)
            {
                _logger?.LogError("GetLinesName: {ex.Message}", ex.Message);
            }

            return new List<string>();
        }

        [HttpPost]
        public Task PostDataToRelatedTables(List<WorkOrder> newWorkOrders)
        {
            using var dbcon = new MainDbContext();

            var existedLines = GetLinesName().Result;
            dbcon.InjectNewWorkPlans(newWorkOrders, existedLines);
                   
           //waking up background task for system calculations
           var productionTimeCalculator = new ProductionTimeScheduler(_logger);   
           productionTimeCalculator.GenerateWorkPlans();
            return Task.CompletedTask;
        }

        [HttpGet]
        public Task DropAllOrders()
        {
            using var dbcon = new MainDbContext();
            dbcon.WorkOrdersFromXL.ExecuteDelete();
            dbcon.ActiveWorkPlans.ExecuteDelete();  
            dbcon.SaveChanges(true);
            return Task.CompletedTask;
        }

        public Task RecreateOrdersTable(WorkOrder wo)
        {
            using var dbcon = new MainDbContext();
            var productionTimeCalculator = new ProductionTimeScheduler(_logger);

            List<WorkOrder> workOrders = new List<WorkOrder>
            {
                wo
            };

            PostDataToRelatedTables(workOrders);
            productionTimeCalculator.RegenerateWorkPlans(wo);
            return Task.CompletedTask;

        }

        }
    }


