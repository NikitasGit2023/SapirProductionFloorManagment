

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SapirProductionFloorManagment.Shared;
using System.Drawing;


namespace SapirProductionFloorManagment.Server
{
    public class MainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Product> Products { get; set; }        
        public DbSet<Customer> Customers { get; set; }      
        public DbSet<WorkOrder> WorkOrdersFromXL { get; set; }    
        public DbSet<LineWorkPlan> ActiveWorkPlans { get; set;}
        public DbSet<LineWorkHours> LinesWorkHours { get; set; }
        public DbSet<AppGeneralData> AppGeneralData { get; set; }
        public string _ConnectionString { get; }
        public ILogger? Logger { get; set; } 

        public MainDbContext()
        {

            var ConnectionString = @"Data Source=DESKTOP-10CMOF7\SQLEXPRESS;Initial Catalog=SapirProductionManagment;User ID=account;Password=3194murkin;Encrypt=False;Connection Timeout=60;";


            _ConnectionString = ConnectionString; 

        }

        //Configuring data connection with SQL Server
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseSqlServer(_ConnectionString);

            }
            catch (Exception ex) 
            {
                Logger?.LogError("OnConfiguring: {ex.Message}", ex.Message);
            }   
            
            
        }
        
        //Set datetime about last workplan calculation
        public async Task SetInfoAboutLastWorkPlanCalculation(DateTime datetime)
        {
            using var dbcon = new MainDbContext();

           
            var existingEntry = await dbcon.AppGeneralData.FirstOrDefaultAsync();

            if (existingEntry != null)
            {
            
                existingEntry.LastWorkPlanCalculation = datetime;

               
                await dbcon.SaveChangesAsync();
            }
        }

        //Get datetime about last workplan calculation
        public Task<DateTime?> GetInfoAboutLastWorkPlanCalculation()
        {

            var lastCalc = AppGeneralData.OrderByDescending(a => a.LastWorkPlanCalculation).FirstOrDefault();


            return Task.FromResult(lastCalc?.LastWorkPlanCalculation);
        }

        //if data base changed , setting data for application functionaliity ( default user, lines, line work hours...)
        public void CreateDefaultDataForAppFunctionallity()
        {
            try
            {

                Users.Add(new User { UserName = "devslave", Password = "devslave", Role = "Developer" });
                SaveChanges();

                Lines.ExecuteDelete();
                SaveChanges();

                LinesWorkHours.ExecuteDelete();
                SaveChanges();

                Lines.Add(new Line { Name = "1", ProductionRate = 200 });
                Lines.Add(new Line { Name = "2", ProductionRate = 200 });
                Lines.Add(new Line { Name = "3", ProductionRate = 200 });
                Lines.Add(new Line { Name = "4", ProductionRate = 200 });
                Lines.Add(new Line { Name = "5", ProductionRate = 200 });
                SaveChanges();

                // Sunday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Sunday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Sunday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Sunday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Sunday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Sunday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Sunday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });

                // Monday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Monday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Monday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Monday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Monday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Monday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Monday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });


                // Tuesday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Tuesday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Tuesday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Tuesday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Tuesday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Tuesday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Tuesday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });

                // Wednesday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Wednesday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Wednesday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Wednesday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Wednesday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Wednesday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Wednesday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });

                // Thursday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Thursday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Thursday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Thursday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Thursday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Thursday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Thursday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });

                // Friday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Friday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Friday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Friday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Friday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Friday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Friday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });

                // Saturday - Shift 08:00 to 16:00, Break 12:00 to 12:30
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "1", WorkDay = "Saturday", ShiftStartWork = "08:00", ShiftEndWork = "16:00", BreakStart = "12:00", BreakEnd = "12:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "2", WorkDay = "Saturday", ShiftStartWork = "08:30", ShiftEndWork = "16:30", BreakStart = "12:30", BreakEnd = "13:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "3", WorkDay = "Saturday", ShiftStartWork = "09:00", ShiftEndWork = "17:00", BreakStart = "13:00", BreakEnd = "13:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "4", WorkDay = "Saturday", ShiftStartWork = "09:30", ShiftEndWork = "17:30", BreakStart = "13:30", BreakEnd = "14:00" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "5", WorkDay = "Saturday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });
                LinesWorkHours.Add(new LineWorkHours { ReferencedToLine = "6", WorkDay = "Saturday", ShiftStartWork = "10:00", ShiftEndWork = "18:00", BreakStart = "14:00", BreakEnd = "14:30" });

                SaveChanges();

            }
            catch(Exception ex)
            {
                Logger?.LogError("CreateDefaultDataForAppFunctionallity: {ex.Messsage}", ex.Message);
            }  
        }

        //Inject new work orders
        public Task InjectWorkOrdersAsync(List<WorkOrder> workOrders)
        {
            try
            {
                for (var i = 0; i < workOrders.Count; i++)
                {
                    var row = workOrders[i];
                    var rowFromDb = WorkOrdersFromXL.FirstOrDefault(e => e.WorkOrderSN == row.WorkOrderSN);

                    if (rowFromDb != null)
                    {
                        continue;
                    }
                    row.QuantityLeft = row.QuantityInKg;
                    WorkOrdersFromXL.Add(row);
                    SaveChanges();

                }

            }
            catch (Exception ex)
            {
                Logger?.LogError("InjectWorkOrdersAsync: {ex.Message}", ex.Message);
            }
            return Task.CompletedTask;
    
        }


        //Update work order status after work plan status updated
        public Task UpdateWorkOrderStatus(LineWorkPlan workPaln)
        {
            try
            {
                var workOrder = WorkOrdersFromXL.Where(e => e.WorkOrderSN == workPaln.WorkOrderSN).FirstOrDefault(); 
                if (workOrder != null) 
                {
                    workOrder.Status = workPaln.Status;
                }
            }catch (Exception ex)
            {
                Logger?.LogError("UpdateWorkOrderStatus: {ex.Message}", ex.Message);
            }
            return Task.CompletedTask;  

        }

        //Inject products relared to work orders
        public Task InjectProducts()
        {
            try 
            {
                var productsToInject = WorkOrdersFromXL.Select(e => e.ProductDesc).ToList();
                foreach (var productName in productsToInject)
                {
                    var p =  Products.Where(e => e.ProductName == productName).FirstOrDefault();
                    if (p != null) 
                    {
                        
                    }
                     Products.AddAsync(new Product
                    {
                        ProductName = productName
                    });
                    
                }
                SaveChanges();

            }
            catch(Exception ex)
            {

            }
            return Task.CompletedTask;
        }

        //Inject customres data related to work orders
        public Task InjectcCustomers()
        {
            try
            {
                var productsToInject = WorkOrdersFromXL.Select(e => e.Comments).ToList();
                foreach (var customerName in productsToInject)
                {
                    var p = Customers.Where(e => e.Name == customerName).FirstOrDefault();
                    if (p != null)
                    {

                    }
                    Customers.AddAsync(new Customer
                    {
                        Name = customerName
                    });

                }
                SaveChanges();

            }
            catch (Exception ex)
            {

            }
            return Task.CompletedTask;
        }

        //Inject new work plans after XL file is uploaded
        public void InjectNewWorkPlans(List<WorkOrder> uploadedWO, List<string> existdLines)
        {
            try
            {

                var workPlan = new List<LineWorkPlan>();

                // Putting work orders inside a work plans dictionary
                Dictionary<WorkOrder, bool> workOrdersDictionary = new();

                foreach (var wo in uploadedWO)
                {
                    workOrdersDictionary.Add(wo, false);
                }


                foreach (var lineName in existdLines)
                {
                    foreach (var workOrder in workOrdersDictionary.Keys)
                    {
                        if ((workOrder.OptionalLine1 == lineName && !string.IsNullOrEmpty(workOrder.OptionalLine1) && workOrder.OptionalLine1 != "")
                            || (workOrder.OptionalLine2 == lineName && !string.IsNullOrEmpty(workOrder.OptionalLine2) && workOrder.OptionalLine2 != ""))
                        {
                            
                            if (workOrdersDictionary[workOrder] == false)
                            {
                        
                                if (workOrder.OptionalLine2 == string.Empty)
                                {

                                    ActiveWorkPlans.Add(new LineWorkPlan
                                    {

                                        RelatedToLine = workOrder.OptionalLine1,
                                        Priority = workOrder.Priority,
                                        DeadLineDateTime = workOrder.DeadLineDateTime,
                                        ProductDesc = workOrder.ProductDesc,
                                        QuantityInKg = workOrder.QuantityInKg,
                                        WorkOrderSN = workOrder.WorkOrderSN,
                                        Comments = workOrder.Comments,
                                        SizeInMicron = workOrder.SizeInMicron,

                                    }); ;
                                    SaveChanges();

                                }
                                else
                                {

                                    ActiveWorkPlans.Add(new LineWorkPlan
                                    {

                                        RelatedToLine = workOrder.OptionalLine1,
                                        Priority = workOrder.Priority,
                                        DeadLineDateTime = workOrder.DeadLineDateTime,
                                        ProductDesc = workOrder.ProductDesc,
                                        QuantityInKg = workOrder.QuantityInKg,
                                        WorkOrderSN = workOrder.WorkOrderSN,
                                        Comments = workOrder.Comments,
                                        SizeInMicron = workOrder.SizeInMicron,

                                    }); ;
                                    SaveChanges();

                                    ActiveWorkPlans.Add(new LineWorkPlan
                                    {

                                        RelatedToLine = workOrder.OptionalLine2,
                                        Priority = workOrder.Priority,
                                        DeadLineDateTime = workOrder.DeadLineDateTime,
                                        ProductDesc = workOrder.ProductDesc,
                                        QuantityInKg = workOrder.QuantityInKg,
                                        WorkOrderSN = workOrder.WorkOrderSN,
                                        Comments = workOrder.Comments,
                                        SizeInMicron = workOrder.SizeInMicron,

                                    }); ;
                                    SaveChanges();


                                }

                                //changing to true inside dictionaty for sign that a specific workOrder is added and avoiding from duplictions
                                workOrdersDictionary[workOrder] = true;

                            }
                        }
                    }
                }

            }
            catch(Exception ex) 
            {
                Logger?.LogError("InjectNewWorkPlans: {ex.Message}", ex.Message);

            }


        }


        public WorkOrder UpdateWorkOrdersRelatedToWorkPlan(LineWorkPlan workPlan)
        {
            var forUpdating = WorkOrdersFromXL.Where(e => e.WorkOrderSN == workPlan.WorkOrderSN).FirstOrDefault();
            if( forUpdating != null ) 
            {
                forUpdating.DeadLineDateTime =(DateTime)workPlan.DeadLineDateTime;
                forUpdating.ProductDesc = workPlan.ProductDesc;
                forUpdating.Priority = workPlan.Priority;
                forUpdating.QuantityInKg = workPlan.QuantityInKg;
                forUpdating.Comments = workPlan.Comments;
                
                WorkOrdersFromXL.Update(forUpdating);
                SaveChanges();
                return forUpdating;
            }
            return null;

        }

      

    }
}
