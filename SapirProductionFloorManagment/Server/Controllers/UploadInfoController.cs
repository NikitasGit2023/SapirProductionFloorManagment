using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Identity.Client;
using Org.BouncyCastle.Crypto;
using SapirProductionFloorManagment.Client.Shared.Tables;
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
        
        public UploadInfoController(ILogger<UploadInfoController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void PostWorkOrdersTable(List<WorkOrdersTableContext> table)
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
        public async Task<List<WorkOrdersTableContext>> GetExistedWorkOrders()
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
            return new List<WorkOrdersTableContext>();

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
        public async Task PostDataToRelatedTables(List<WorkOrdersTableContext> linesSchedule)
        {
            try
            {
                using var dbcon = new MainDbContext();
                var scheduleToDb = new List<LinesScheduleTableContext>();         

                // Putting work orders inside a lines schedule dictionary
                Dictionary<WorkOrdersTableContext, bool> workOrdersDictionary = new();

                foreach (var lineSchedule in linesSchedule) 
                {
                    workOrdersDictionary.Add(lineSchedule, false);
                }

                // Fetching lines name
                var linesName = await GetLinesName();

                foreach (var lineName in linesName)
                {
                    foreach (var workOrder in workOrdersDictionary.Keys)
                    {
                        if ((workOrder.OptionalLine1 == lineName && !string.IsNullOrEmpty(workOrder.OptionalLine1) && workOrder.OptionalLine1 != "0")
                            || (workOrder.OptionalLine2 == lineName && !string.IsNullOrEmpty(workOrder.OptionalLine2) && workOrder.OptionalLine2 != "0"))
                        {

                            if (workOrdersDictionary[workOrder] == false)
                            {
                                dbcon.LinesWorkSchedule.Add(new LinesScheduleTableContext
                                {
                                    RelatedToLine = lineName,
                                    QuantityInKg = workOrder.Quantity,
                                    WorkOrderSN = workOrder.WorkOrderSN,
                                    Comments = workOrder.Comments,
                                    SizeInMicron = workOrder.SizeInMicron,

                                });
                                dbcon.SaveChanges();  
                                
                                //changing to true inside dictionaty for sign that a specific workOrder is added and avoiding from duplictions
                                workOrdersDictionary[workOrder] = true;

                            }

                     
                        }
                    }
                }

                //waking up background task for system calculations
            }
            catch (Exception ex)
            {
                _logger?.LogError("PostDataToRelatedTables: {ex.Message}", ex.Message);
            }
        }

        }
    }


