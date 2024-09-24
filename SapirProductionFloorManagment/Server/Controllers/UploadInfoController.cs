using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
            return await Task.FromResult("המידע עודכן בהצלחה");

        }

        [HttpPost]
        public async Task<string> DeleteOrder(WorkOrder wo)
        {
            if (wo == null)
            {
                return await Task.FromResult("");
            }
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Remove(wo);
                dbcon.SaveChanges();

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
                dbcon.Add(wo);
                dbcon.SaveChanges();

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
        public async Task PostDataToRelatedTables(List<WorkOrder> newWorkOrders)
        {
            using var dbcon = new MainDbContext();

            var existedLines = GetLinesName().Result;
            dbcon.InjectNewWorkPlans(newWorkOrders, existedLines);
                   
           //waking up background task for system calculations
           var productionTimeCalculator = new ProductionTimeScheduler(_logger);   
           productionTimeCalculator.WakeUpBackGroundService();

        }

        [HttpGet]
        public async Task RecreateOrders()
        {
            using var dbcon = new MainDbContext();
            var tempPlans = dbcon.ActiveWorkPlans.ToList();
            dbcon.ActiveWorkPlans.RemoveRange(tempPlans);
            dbcon.SaveChanges(true);

            var workOrders = dbcon.WorkOrdersFromXL.ToList();         
            await PostDataToRelatedTables(workOrders);                        
        }

        }
    }


