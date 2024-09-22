using Microsoft.EntityFrameworkCore.Internal;
using NPOI.SS.Formula.Functions;
using OfficeOpenXml.Export.ToCollection;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.BackgroundTasks
{
    public class ProductionTimeScheduler
    {
        public ILogger? Logger { get; set; }

        private DataConverter _converter;
        private TimeSchedulerHelper _schedulerHelper;
        private List<WorkOrder> _workOrders;
        private List<LineWorkPlan> _workPlans;     
        private List<LineWorkHours> _linesWorkHours;
        private MainDbContext _dbcon;   


        public ProductionTimeScheduler(ILogger logger)
        {
            Logger = logger;
            _converter = new DataConverter();
            _schedulerHelper = new TimeSchedulerHelper();
            _dbcon = new MainDbContext();   
        }

        private void FetchRelatedTables()
        {
            try
            {
                using var dbcon = new MainDbContext();
                _workOrders = dbcon.WorkOrdersFromXL.ToList();
                _workPlans = dbcon.ActiveWorkPlans.ToList();
                _linesWorkHours = dbcon.LinesWorkHours.ToList();

            }
            catch (Exception ex)
            {
                Logger?.LogError("FetchRelatedTables: {ex.Message}", ex.Message);
            }       
        }
        public void WakeUpBackGroundService()
        {
            var lineNum = 0;
            FetchRelatedTables();

            try
            {
            
                using var dbcon = new MainDbContext();
   
                for (int i = 0; i < _workPlans.Count(); i++)
                {

                    var lineWorkPlan = _workPlans[i];


                    int.TryParse(lineWorkPlan.RelatedToLine, out lineNum);

                    var productionRate = _schedulerHelper.SetProductionRate(lineNum, lineWorkPlan.SizeInMicron);
    
                    lineWorkPlan.WorkDuraion = _schedulerHelper.SetWorkDuration(lineWorkPlan.QuantityInKg, lineWorkPlan);
              
                    lineWorkPlan.FormatedWorkDuration = _converter.ConvertToTimeString(lineWorkPlan.WorkDuraion);

                    lineWorkPlan.LeftToFinish = lineWorkPlan.WorkDuraion;

                    var workOrder = _workOrders.Where(e => e.WorkOrderSN == lineWorkPlan.WorkOrderSN).First();
                    lineWorkPlan.DeadLineDateTime = workOrder.CompletionDate; 

                    dbcon.ActiveWorkPlans.Update(lineWorkPlan);
                    dbcon.SaveChanges();

                }

                //generating a work plan after , so how can I inject a dictionary of breaks here? 
                var breaksDictionary = _schedulerHelper.BuildBreakDictionary(_linesWorkHours);
                GenerateWorkPlan(breaksDictionary);

            }
            catch (Exception ex)
            {
                Logger?.LogError("WakeUpBackGroundService: {ex.Message}", ex.Message);
            }

        }
        private void GenerateWorkPlan(Dictionary<string, TimeSpan> breaks)
        {
            using var dbcon = new MainDbContext();

            var sortedWorkOrders = _workOrders.OrderByDescending(w => w.Priority).ThenBy(w => w.CompletionDate).ToList();
            var workPlans = dbcon.ActiveWorkPlans.ToList();
     
            foreach (var workOrder in sortedWorkOrders)
            {
                
                int availibleLine = _schedulerHelper.FindAvailableLine(workOrder, _linesWorkHours, workPlans);
                 if (availibleLine != -1)
                {
                    var workDuration = dbcon.ActiveWorkPlans.Where(e => e.WorkOrderSN == workOrder.WorkOrderSN)
                                                              .First().WorkDuraion;

                    var workHours = _linesWorkHours.First(e => e.ReferencedToLine == availibleLine.ToString() 
                                      && e.WorkDay == DateTime.Now.DayOfWeek.ToString());

                 
                    var workPlan = dbcon.ActiveWorkPlans.Where(e => e.WorkOrderSN == workOrder.WorkOrderSN 
                                        && e.RelatedToLine == availibleLine.ToString()).First();

                    if (workPlan.LeftToFinish == 0)//means that compleated
                        continue;

                    var workBreaks = _schedulerHelper.BuildBreakDictionary(dbcon.LinesWorkHours.ToList());
                    SetWorkStartAndEndTime(workPlan, workDuration, workPlans, workHours, workBreaks);
                    dbcon.Update(workPlan);
                    dbcon.SaveChanges();                     
                   
                }
            }

        }
        private void RegenerateWorkPlan(Dictionary<string, TimeSpan> breaks)
        {
            //TODO
        }        
        private void SetWorkStartAndEndTime(LineWorkPlan workPlan, double workDurationHours, List<LineWorkPlan> lineWorkPlans, LineWorkHours workHours, Dictionary<string, TimeSpan> breaks)
        {
            try
            {
                // Parse the shift's start and end time
                DateTime shiftStart = DateTime.Parse(workHours.ShiftStartWork);
                DateTime shiftEnd = DateTime.Parse(workHours.ShiftEndWork);
                TimeSpan workDuration = TimeSpan.FromHours(workDurationHours);

                // Find the last work plan for the line and determine the next available start time
                var lastPlan = lineWorkPlans
                    .Where(l => l.RelatedToLine == workPlan.RelatedToLine.ToString())
                    .OrderByDescending(l => l.EndWork)
                    .FirstOrDefault();

                // Default to shift start if no prior plan exists
                DateTime nextAvailableStart = lastPlan != null && lastPlan.EndWork != null ? (DateTime)lastPlan.EndWork : shiftStart;

                // Ensure the work starts within the shift
                if (nextAvailableStart >= shiftEnd)
                {
                    Console.WriteLine("Work cannot be scheduled after the shift ends.");
                    return; // Do nothing if work starts after the shift end
                }

                // Calculate the available time until the shift ends
                TimeSpan availableShiftTime = shiftEnd - nextAvailableStart;

                // Adjust for breaks
                DateTime adjustedEndForBreaks = _schedulerHelper.AdjustForBreaks(nextAvailableStart, availableShiftTime, workHours, breaks);
                availableShiftTime = adjustedEndForBreaks - nextAvailableStart; // Recalculate available shift time after breaks

                // Determine if work can be completed within the available shift time
                if (availableShiftTime >= workDuration)
                {
                    // Full work duration can be scheduled within this shift
                    workPlan.StartWork = nextAvailableStart;
                    workPlan.EndWork = _schedulerHelper.AdjustForBreaks(nextAvailableStart, workDuration, workHours, breaks);
                    workPlan.LeftToFinish = 0; // No remaining work
                }
                else
                {
                    // Partial work will be scheduled in the current shift
                    workPlan.StartWork = nextAvailableStart;

                    // Calculate the remaining work that will be left for the next shift
                    TimeSpan remainingWorkTime = workDuration - availableShiftTime;

                    // Set end time for this shift, adjusted for any breaks
                    DateTime shiftEndWithBreaks = _schedulerHelper.AdjustForBreaks(nextAvailableStart, availableShiftTime, workHours, breaks);
                    workPlan.EndWork = shiftEndWithBreaks;

                    // Save remaining hours for the next shift
                    workPlan.LeftToFinish = remainingWorkTime.TotalHours;

                    //add uncompleated work plans to recheaduled work plans table
                    _schedulerHelper.RecheduleWorkPlan(workPlan);

                    _dbcon.SaveChanges();
                   
                    
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating start and end time with breaks: {ex.Message}");
            }
        }

        









    }
}

