

using Microsoft.EntityFrameworkCore;
using SapirProductionFloorManagment.Shared;


namespace SapirProductionFloorManagment.Server
{
    public class MainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Product> Products { get; set; }        
        public DbSet<WorkOrder> WorkOrdersFromXL { get; set; }    
        public DbSet<LineWorkPlan> ActiveWorkPlans { get; set;}

        //public DbSet<LineWorkPlan> RecheduledWorkPlans { get; set; }    
        public DbSet<LineWorkHours> LinesWorkHours { get; set; }
        public DbSet<AppGeneralData> AppGeneralData { get; set; }
        public string _ConnectionString { get; }
        public ILogger? Logger { get; set; } 

        public MainDbContext()
        {
         
            var ConnectionString = @"Data Source=DESKTOP-10CMOF7\SQLEXPRESS;Initial Catalog=SapirProductionManagment;User ID=account;Password=3194murkin;Encrypt=False";
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


        public void CreateDefaultDataForAppFunctionallity()
        {
            try
            {

                Users.Add(new User { FullName = "devslave", Password = "devslave", Role = "Developer", JobTitle = "Developer" });
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
                            //relevant
                            if (workOrdersDictionary[workOrder] == false)
                            {
                        
                                if (workOrder.OptionalLine2 == string.Empty)
                                {

                                    ActiveWorkPlans.Add(new LineWorkPlan
                                    {

                                        RelatedToLine = workOrder.OptionalLine1,
                                        Priority = workOrder.Priority,
                                        DeadLineDateTime = workOrder.CompletionDate,
                                        Description = workOrder.ProductDesc,
                                        QuantityInKg = workOrder.Quantity,
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
                                        DeadLineDateTime = workOrder.CompletionDate,
                                        Description = workOrder.ProductDesc,
                                        QuantityInKg = workOrder.Quantity,
                                        WorkOrderSN = workOrder.WorkOrderSN,
                                        Comments = workOrder.Comments,
                                        SizeInMicron = workOrder.SizeInMicron,

                                    }); ;
                                    SaveChanges();

                                    ActiveWorkPlans.Add(new LineWorkPlan
                                    {

                                        RelatedToLine = workOrder.OptionalLine2,
                                        Priority = workOrder.Priority,
                                        DeadLineDateTime = workOrder.CompletionDate,
                                        Description = workOrder.ProductDesc,
                                        QuantityInKg = workOrder.Quantity,
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



    }
}
