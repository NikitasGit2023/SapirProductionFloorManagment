using Microsoft.EntityFrameworkCore.Internal;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.BackgroundTasks
{
    public class ProductionTimeScheduler
    {
        //private readonly ILogger<ProductionTimeScheduler> _logger;      
        //public ProductionTimeScheduler(ILogger<ProductionTimeScheduler> logger) 
        //{
        //    _logger = logger;   
        //}
        public ILogger? Logger { get; set; }     

        public ProductionTimeScheduler(ILogger logger)
        {
            Logger = logger;
           
           
        }

        public void WakeUpBackGroundService()
        {
            var lineNum = 0;

            try
            {
                //fetching from db lines work schedule
                using var dbcon = new MainDbContext();
                var workOrders = dbcon.WorkOrdersFromXL.ToList();
                var schdule = dbcon.LinesWorkSchedule.ToList();
                var linesWorkHours = dbcon.LinseWorkHours.ToList(); 

                //setting data for each lines using logic
                for (int i = 0; i < schdule.Count(); i++)
                {

                    var lineWorkPlan = schdule[i];
                    int.TryParse(lineWorkPlan.RelatedToLine, out lineNum);

                    //calculating production rate
                    var productionRate = CalculateProductionRate(lineNum, lineWorkPlan.SizeInMicron);

                    //setting work duration for each workplan than related to specific line
                    lineWorkPlan.WorkDuraion = CalculateWorkDuration(lineWorkPlan.QuantityInKg, lineWorkPlan);

                    //update db with new line new LineWorkPlan row
                    lineWorkPlan.FormatedWorkDuration = ConvertDoubleToTimeFormat(lineWorkPlan.WorkDuraion);

                    dbcon.LinesWorkSchedule.Update(lineWorkPlan);
                    dbcon.SaveChanges();

                }

                //generating a work plan after , so how can I inject a dictionary of breaks here? 
                var breaksDictionary = BuildBreakDictionary(linesWorkHours);
                GenerateWorkPlan(workOrders, linesWorkHours, breaksDictionary);

            }
            catch (Exception ex)
            {
                Logger?.LogError("WakeUpBackGroundService: {ex.Message}", ex.Message);
            }

        }
        private Dictionary<string, TimeSpan> BuildBreakDictionary(List<LineWorkHours> linesWorkHours)
        {
            // The key is a combination of LineNumber and WorkDay (e.g., "1_Sunday", "2_Monday")
            var breakDictionary = new Dictionary<string, TimeSpan>();

            foreach (var lineWorkHour in linesWorkHours)
            {
                // Only add if both break start and end are specified
                if (!string.IsNullOrEmpty(lineWorkHour.BreakStart) && !string.IsNullOrEmpty(lineWorkHour.BreakEnd))
                {
                    string key = $"{lineWorkHour.ReferencedToLine}_{lineWorkHour.WorkDay}";

                    TimeSpan breakStart = TimeSpan.Parse(lineWorkHour.BreakStart);
                    TimeSpan breakEnd = TimeSpan.Parse(lineWorkHour.BreakEnd);

                    breakDictionary[key + "_Start"] = breakStart;
                    breakDictionary[key + "_End"] = breakEnd;
                }
            }

            return breakDictionary;
        }
        public void GenerateWorkPlan(List<WorkOrder> workOrders, List<LineWorkHours> lineWorkHours, Dictionary<string, TimeSpan> breaks)
        {
            using var dbcon = new MainDbContext();

            // Sorting work orders by Priority and DueDate
            var sortedWorkOrders = workOrders.OrderByDescending(w => w.Priority).ThenBy(w => w.CompletionDate).ToList();
            var linesWorkPlans = dbcon.LinesWorkSchedule.ToList();

            foreach (var workOrder in sortedWorkOrders)
            {
                //finding avalible line for work order
                int selectedLine = FindAvailableLine(workOrder, lineWorkHours, linesWorkPlans);
                 if (selectedLine != -1)
                {
                    var workDuration = dbcon.LinesWorkSchedule.Where(e => e.WorkOrderSN == workOrder.WorkOrderSN).First().WorkDuraion;
                    var workHours = lineWorkHours.First(l => l.Id == selectedLine);

                    DateTime startWork = GetNextAvailableStartTime(selectedLine, workDuration, linesWorkPlans, workHours);
                    DateTime endWork = CalculateWorkEndTime(startWork,workDuration, workHours, breaks);

                    var lineWorkPlan = dbcon.LinesWorkSchedule.Where(e => e.WorkOrderSN == workOrder.WorkOrderSN).First();
                    lineWorkPlan.StartWork = startWork; 
                    lineWorkPlan.EndWork = endWork;

                    dbcon.Update(lineWorkPlan);
                    dbcon.SaveChanges();    
                   
                }
            }

        }
        private int CalculateProductionRate(int lineNum, int sizeInMicron)
        {
            var productRate = 0;

            switch (lineNum)
            {
                case 4:
                    switch (sizeInMicron)
                    {
                        case 12:
                            productRate = 200;
                            break;
                        case 13:
                            productRate = 200;
                            break;
                        case 14:
                            productRate = 200;
                            break;
                        case 15:
                            productRate = 450;
                            break;
                        case 16:
                            productRate = 450;
                            break;
                        case 17:
                            productRate = 620;
                            break;
                        case 18:
                            productRate = 620;
                            break;
                        case 19:
                            productRate = 620;
                            break;
                        case 20:
                            productRate = 800;
                            break;
                        case 21:
                            productRate = 800;
                            break;
                        case 22:
                            productRate = 870;
                            break;
                        case 23:
                            productRate = 870;
                            break;
                        case 24:
                            productRate = 870;
                            break;
                        case 25:
                            productRate = 900;
                            break;
                        case 26:
                            productRate = 900;
                            break;
                        case 27:
                            productRate = 900;
                            break;
                        case 28:
                            productRate = 900;
                            break;
                        case 29:
                            productRate = 900;
                            break;
                        case 30:
                            productRate = 950;
                            break;
                        case 31:
                            productRate = 950;
                            break;
                        case 32:
                            productRate = 950;
                            break;
                        case 33:
                            productRate = 950;
                            break;
                        case 34:
                            productRate = 950;
                            break;
                        case 35:
                            productRate = 950;
                            break;
                    }
                    break;

                case 5:
                    switch (sizeInMicron)
                    {
                        case 12:
                            productRate = 900;
                            break;
                        case 13:
                            productRate = 900;
                            break;
                        case 14:
                            productRate = 900;
                            break;
                        case 15:
                            productRate = 900;
                            break;
                        case 16:
                            productRate = 900;
                            break;
                        case 17:
                            productRate = 1000;
                            break;
                        case 18:
                            productRate = 1000;
                            break;
                        case 19:
                            productRate = 1200;
                            break;
                        case 20:
                            productRate = 1350;
                            break;
                        case 21:
                            productRate = 1350;
                            break;
                        case 22:
                            productRate = 1350;
                            break;
                        case 23:
                            productRate = 1500;
                            break;
                        case 24:
                            productRate = 1500;
                            break;
                        case 25:
                            productRate = 1600;
                            break;
                        case 26:
                            productRate = 1600;
                            break;
                        case 27:
                            productRate = 1600;
                            break;
                        case 28:
                            productRate = 1600;
                            break;
                        case 29:
                            productRate = 1600;
                            break;
                        case 30:
                            productRate = 1700;
                            break;
                        case 31:
                            productRate = 0;
                            break;
                        case 32:
                            productRate = 0;
                            break;
                        case 33:
                            productRate = 0;
                            break;
                        case 34:
                            productRate = 0;
                            break;
                        case 35:
                            productRate = 1800;
                            break;
                    }
                    break;
            }
            return productRate;
        }
        private double CalculateWorkDuration(int quantityInKg, LineWorkPlan lineSchedule)
        {
            var duration = 0.0; 
            int relatedLine;

            if (int.TryParse(lineSchedule.RelatedToLine, out relatedLine))
            {
                if (relatedLine == 4 || relatedLine == 5)
                {
        
                    var prodRate = CalculateProductionRate(relatedLine, lineSchedule.SizeInMicron);
                
                    duration = (double)quantityInKg / prodRate / 24;  
                    return duration;
                }
            }

            using var dbcon = new MainDbContext();

            var prodRateAnother = dbcon.Lines.Where(e => e.Name == lineSchedule.RelatedToLine).Select(e => e.ProductionRate).First();
            duration = (double)quantityInKg / prodRateAnother / 24; 
            return duration;
        }
        public string ConvertDoubleToTimeFormat(double time)
        {
            // Get the integer part of the time, which represents hours
            int hours = (int)time;

            // Get the fractional part of the time, which represents minutes (in fractions of an hour)
            double fractionalPart = time - hours;
            int minutes = (int)(fractionalPart * 60); // Convert fractional hours to minutes

            // Format the hours with parentheses and two digits
            string formattedHours = $"({hours:D2})";

            // Format the minutes and seconds in two digits
            string formattedMinutes = $"{minutes:D2}:00";

            // Combine everything into the final string
            return $"{formattedHours}+ {formattedMinutes}";
        }
        private int FindAvailableLine(WorkOrder workOrder, List<LineWorkHours> lineWorkHours, List<LineWorkPlan> lineWorkPlans)
        {
            // Check if OptionalLine1 or OptionalLine2 are available.
            var linesToCheck = new List<string?> { workOrder.OptionalLine1, workOrder.OptionalLine2 }.Where(l => l != null);

            foreach (var line in linesToCheck)
            {
                var lineWorkHour = lineWorkHours.FirstOrDefault(l => l.Id.ToString() == line && l.ShiftStartWork != "Unspecified" && l.ShiftEndWork != "Unspecified");
                if (lineWorkHour != null)
                {
                    return lineWorkHour.Id;
                }
            }

            return -1; // No available line found.
        }
        private DateTime GetNextAvailableStartTime(int lineId, double workDurationHours, List<LineWorkPlan> lineWorkPlans, LineWorkHours workHours)
        {
            var nextAvailableStart = DateTime.Now;
            try
            {
                // Parse shift start and end times
                DateTime shiftStart = DateTime.Parse(workHours.ShiftStartWork);
                DateTime shiftEnd = DateTime.Parse(workHours.ShiftEndWork);

                // Convert work duration (hours) into TimeSpan
                TimeSpan workDuration = TimeSpan.FromHours(workDurationHours);

                // Find the next available time after the last work plan
                var lastPlan = lineWorkPlans.Where(l => l.RelatedToLine == lineId.ToString()).OrderByDescending(l => l.EndWork).FirstOrDefault();
                nextAvailableStart = lastPlan != null ? lastPlan.EndWork : shiftStart;

                //if (nextAvailableStart.Add(workDuration) > shiftEnd)
                //{
                //    throw new Exception("Not enough time in the shift to schedule this work order.");
                //}

            }
            catch(Exception ex)
            {

            }

            return nextAvailableStart;
        }
        private DateTime CalculateWorkEndTime(DateTime workStart, double workDurationHours, LineWorkHours workHours, Dictionary<string, TimeSpan> breaks)
        {
            var workEnd = DateTime.Now;

            try
            {
                DateTime breakStart = string.IsNullOrEmpty(workHours.BreakStart) ? DateTime.MaxValue : DateTime.Parse(workHours.BreakStart);
                DateTime breakEnd = string.IsNullOrEmpty(workHours.BreakEnd) ? DateTime.MaxValue : DateTime.Parse(workHours.BreakEnd);

                // Convert work duration (hours) into TimeSpan
                TimeSpan workDuration = TimeSpan.FromHours(workDurationHours);
                 workEnd = workStart.Add(workDuration);

                // If the break occurs within the working period, adjust the end time
                if (workStart < breakStart && workEnd > breakStart)
                {
                    TimeSpan breakDuration = breakEnd - breakStart;
                    workEnd = workEnd.Add(breakDuration);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return workEnd;
        }
    }
}

