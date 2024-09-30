
using ICSharpCode.SharpZipLib;
using SapirProductionFloorManagment.Shared;
using System.Linq.Dynamic.Core;

namespace SapirProductionFloorManagment.Server.BackgroundTasks
{
    public class ProductionTimeScheduler 
    {
        public ILogger? Logger { get; set; }

        private DataConverter _converter;
        private TimeSchedulerHelper _schedulerHelper;
        private List<WorkOrder> _workOrders;
        private List<LineWorkHours> _workHours;
        private const string REACHEDULED = "REACHEDULED";
        private const string IN_PROGGRESS = "IN PROGRESS";


        public ProductionTimeScheduler(ILogger logger)
        {
            Logger = logger;
            _converter = new DataConverter();
            _schedulerHelper = new TimeSchedulerHelper 
            {
                Logger = Logger
            };
        }

        private void SetBasicDataToWorkPlans(List<LineWorkPlan> workPlans)
        {
            var lineNum = 0;
            using var dbcon = new MainDbContext();
            _workOrders = dbcon.WorkOrdersFromXL.ToList();
            _workHours = dbcon.LinesWorkHours.ToList();


            for (int i = 0; i < workPlans.Count(); i++)
            {

                int.TryParse(workPlans[i].RelatedToLine.ToString(), out lineNum);
                var productionRate = _schedulerHelper.SetProductionRate(lineNum, workPlans[i].SizeInMicron);
                workPlans[i].WorkDuraion = _schedulerHelper.SetWorkDuration(workPlans[i].QuantityInKg, workPlans[i]);
                workPlans[i].FormatedWorkDuration = _converter.ConvertToTimeString(workPlans[i].WorkDuraion);
                var workOrder = dbcon.WorkOrdersFromXL.Where(e => e.WorkOrderSN == workPlans[i].WorkOrderSN).First();
                workPlans[i].DeadLineDateTime = workOrder.DeadLineDateTime;
                workPlans[i].LeftToFinish = workPlans[i].WorkDuraion;
                workPlans[i].StartWork = null;
                workPlans[i].EndWork = null;

                try
                {
                    dbcon.ActiveWorkPlans.Update(workPlans[i]);
                    dbcon.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger?.LogError("WakeUpBackGroundService: {ex.Message}", ex.Message);
                }

            }

        }


        public void GenerateWorkPlans()
        {
            using var dbcon = new MainDbContext();
            var plansFromDb = dbcon.ActiveWorkPlans.ToList();
            SetBasicDataToWorkPlans(plansFromDb);
            var breaksDictionary = _schedulerHelper.BuildBreakDictionary(_workHours);

          
            var lines = dbcon.ActiveWorkPlans.Select(e => e.RelatedToLine)
                                             .Distinct()
                                             .ToList(); 


            var sortedWO = _workOrders.OrderBy(w => w.Priority)
                                      .ThenBy(w => w.DeadLineDateTime)
                                      .ToList(); 

            var workPlans = dbcon.ActiveWorkPlans.ToList();


                foreach (var workOrder in sortedWO)
                {
                        
                        var revelantPlans = dbcon.ActiveWorkPlans.Where(e=> e.WorkOrderSN == workOrder.WorkOrderSN).ToArray(); 

                        if (revelantPlans.Count() != 0)
                        {    
                           SetWorkStartAndEndTime(revelantPlans, breaksDictionary);
                        }


                    }
             Logger?.LogInformation("Generation of work plans is compleated sucsesfully");
                }


        public void RegenerateWorkPlans(WorkOrder wo)
        {
            
            using var dbcon = new MainDbContext();
            

            var relatedWorkPlans = dbcon.ActiveWorkPlans.Where(e => e.RelatedToLine == wo.OptionalLine1).ToList();
            var relatedWorkPlansOther = dbcon.ActiveWorkPlans.Where(e => e.RelatedToLine == wo.OptionalLine2).ToList();

            relatedWorkPlans.AddRange(relatedWorkPlansOther);
            SetBasicDataToWorkPlans(relatedWorkPlans);
            

            var lines = dbcon.ActiveWorkPlans.Select(e => e.RelatedToLine)
                                             .Distinct()
                                             .ToList();

          
            var  sortedWO = _workOrders
                .Where(w => relatedWorkPlans.Any(r => r.WorkOrderSN == w.WorkOrderSN))  
                .OrderBy(w => w.Priority)  
                .ThenBy(w => w.DeadLineDateTime)  
                .ToList();


            var breaksDictionary = _schedulerHelper.BuildBreakDictionary(_workHours);


                foreach (var workOrder in sortedWO)
                {
                    
                    var revelantPlans = dbcon.ActiveWorkPlans.Where(e => e.WorkOrderSN == workOrder.WorkOrderSN).ToArray();
                    
                    if (revelantPlans.Count() != 0)
                    {
                        SetWorkStartAndEndTime(revelantPlans, breaksDictionary);
                    }


            }
            Logger?.LogInformation("Regeneration work plans in compleated sucsesfully");
        }

      
        private void SetWorkStartAndEndTime(LineWorkPlan[] relevantPlans, Dictionary<string, TimeSpan> breaks)
        {
            if (relevantPlans.Length == 0) return;

            using var dbcon = new MainDbContext { Logger = Logger };

            try
            {
                // Fetch work hours for the first and second plans
                var firstWorkHours = GetWorkHours(relevantPlans[0], dbcon);
                var secondWorkHours = relevantPlans.Length == 2 ? GetWorkHours(relevantPlans[1], dbcon) : null;

                if (firstWorkHours == null || (relevantPlans.Length == 2 && secondWorkHours == null)) return;

                // Get next available start times
                var firstNextStart = GetNextAvailableStart(relevantPlans[0], firstWorkHours, dbcon);
                var secondNextStart = relevantPlans.Length == 2 ? GetNextAvailableStart(relevantPlans[1], secondWorkHours, dbcon) : null;

                if (firstNextStart == null && secondNextStart == null) return;

                // Determine earliest plan
                var earliestPlan = DetermineEarliestPlan(relevantPlans, firstNextStart, secondNextStart);

                if (earliestPlan == null) return;

                // Calculate work duration and shift end
                var shiftEnd = earliestPlan.RelatedToLine == relevantPlans[0].RelatedToLine ? DateTime.Parse(firstWorkHours.ShiftEndWork) : DateTime.Parse(secondWorkHours.ShiftEndWork);
                DateTime earliestStart = earliestPlan.RelatedToLine == relevantPlans[0].RelatedToLine ? (DateTime)firstNextStart : (DateTime)secondNextStart;

                // Adjust for breaks and set work times 
                 earliestPlan =  CalculateAndSetWorkTimes(earliestPlan, earliestStart, shiftEnd, firstWorkHours, secondWorkHours, breaks, dbcon);

                if(earliestPlan.StartWork == null && relevantPlans[0] != null)
                {
                    earliestPlan = relevantPlans[0];
                    earliestPlan.StartWork = secondNextStart;
                    CalculateAndSetWorkTimes(earliestPlan, earliestStart, shiftEnd, firstWorkHours, secondWorkHours, breaks, dbcon);
                }

                // if shift start is null try adjust work on second work plan 

                var forRemoving = dbcon.ActiveWorkPlans.Where(e => e.WorkOrderSN == earliestPlan.WorkOrderSN
                                            && e.Id != earliestPlan.Id).FirstOrDefault();

                if (forRemoving == null)
                {
                    return;
                }
                else
                {
                    dbcon.ActiveWorkPlans.Remove(forRemoving);
                    dbcon.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                Logger?.LogError("SetWorkStartAndEndTime: {0}", ex.Message);
            }
        }

        private LineWorkHours GetWorkHours(LineWorkPlan plan, MainDbContext dbcon)
        {
            return _workHours.FirstOrDefault(e => e.ReferencedToLine == plan.RelatedToLine && e.WorkDay == DateTime.Now.DayOfWeek.ToString());
        }

        private DateTime? GetNextAvailableStart(LineWorkPlan plan, LineWorkHours workHours, MainDbContext dbcon)
        {
            var lastCompletedPlan = dbcon.ActiveWorkPlans.Where(e => e.RelatedToLine == plan.RelatedToLine && e.EndWork != null)
                                          .OrderByDescending(e => e.EndWork)
                                          .FirstOrDefault();

            if (lastCompletedPlan == null)
            {
                lastCompletedPlan = dbcon.ActiveWorkPlans.Where(e => e.RelatedToLine == plan.RelatedToLine && e.EndWork == null)
                                                         .OrderByDescending(e => e.EndWork)
                                                         .FirstOrDefault();
            }

            return lastCompletedPlan?.EndWork ?? DateTime.Parse(workHours.ShiftStartWork);
        }

        private LineWorkPlan DetermineEarliestPlan(LineWorkPlan[] plans, DateTime? firstNextStart, DateTime? secondNextStart)
        {
            LineWorkPlan? earliestPlan = null;
            switch (plans.Length) 
            {
                case 0:
                    return earliestPlan;
                case 1: 
                    earliestPlan = plans[0];
                    return earliestPlan;
                case 2:
                    if (firstNextStart <= secondNextStart)
                    {
                        earliestPlan = plans[0];
                        return earliestPlan;

                    }
                    earliestPlan = plans[1];
                    return earliestPlan;
            }
            return earliestPlan;
        }
        

        private LineWorkPlan CalculateAndSetWorkTimes(LineWorkPlan plan, DateTime workStart, DateTime shiftEnd, LineWorkHours firstWorkHours,
                                                        LineWorkHours secondWorkHours, Dictionary<string, TimeSpan> breaks, MainDbContext dbcon)
        {
            LineWorkHours selectedWorkHours = plan.RelatedToLine == firstWorkHours.ReferencedToLine ? firstWorkHours : secondWorkHours;

            TimeSpan availableShiftTime = shiftEnd - workStart;
            DateTime adjustedEndForBreaks = _schedulerHelper.AdjustForBreaks(workStart, availableShiftTime, selectedWorkHours, breaks);  // Original signature preserved
            availableShiftTime = adjustedEndForBreaks - workStart;

            if (availableShiftTime >= TimeSpan.FromHours(plan.WorkDuraion)) // still time to complete work
            {
                plan.EndWork = _schedulerHelper.AdjustForBreaks(workStart, TimeSpan.FromHours(plan.WorkDuraion), selectedWorkHours, breaks);
                plan.StartWork = workStart;
                plan.Status = IN_PROGGRESS;
                plan.LeftToFinish = 0;
            }
            else
            {
                plan.StartWork = null;
                plan.EndWork = null;
                TimeSpan remainingWorkTime = TimeSpan.FromHours(plan.WorkDuraion) - availableShiftTime;
                //ReachduleWorkPlan(remainingWorkTime, availableShiftTime, plan, workStart, shiftEnd);
            }

            // Update or insert the plan
            var existingPlan = dbcon.ActiveWorkPlans.FirstOrDefault(e => e.Id == plan.Id);
            if (existingPlan != null)
            {
                existingPlan.StartWork = plan.StartWork;
                existingPlan.EndWork = plan.EndWork;
                existingPlan.LeftToFinish = plan.LeftToFinish;
                existingPlan.Status = plan.Status;
                dbcon.ActiveWorkPlans.Update(existingPlan);
            }
            else
            {
                dbcon.ActiveWorkPlans.Add(plan);
                
            }
            dbcon.SaveChanges();
            return existingPlan;

        }

        private void ReachduleWorkPlan(TimeSpan remainingWorkTime, TimeSpan availableShiftTime, LineWorkPlan workPlan, DateTime startWork, DateTime shiftEnd)
        {

            using var dbcon = new MainDbContext();
            try
            {
       
                if (remainingWorkTime > availableShiftTime && availableShiftTime.TotalMinutes > 0)
                {
                    workPlan.StartWork = startWork;
                    workPlan.EndWork = shiftEnd;

                    var workDuration = remainingWorkTime.TotalHours - availableShiftTime.TotalHours;
                    var leftToFinish = remainingWorkTime.TotalHours - availableShiftTime.TotalHours;

                    dbcon.ActiveWorkPlans.Add(new LineWorkPlan
                    {
                        FormatedLeftToFinish = _converter.ConvertToTimeString(leftToFinish),
                        FormatedWorkDuration = _converter.ConvertToTimeString(leftToFinish),
                        LeftToFinish = leftToFinish,
                        WorkDuraion = workDuration,
                        WorkOrderSN = workPlan.WorkOrderSN,
                        EndWork = null,
                        StartWork = null,
                        Comments = workPlan.Comments,
                        DeadLineDateTime = workPlan.DeadLineDateTime,
                        ProductDesc = workPlan.ProductDesc,
                        Priority = workPlan.Priority,   
                        QuantityInKg = workPlan.QuantityInKg,
                        RelatedToLine = workPlan.RelatedToLine,
                        SizeInMicron = workPlan.SizeInMicron,
                        Status = REACHEDULED
                    });
                    dbcon.SaveChanges();

                }
                else
                {
                    workPlan.StartWork = null;
                    workPlan.EndWork = null;
                    workPlan.Status = REACHEDULED;
                }


            }
            catch (Exception ex)
            {
                Logger?.LogError("ReachduleWorkPlan: {ex.Message}", ex.Message);
            }

        }

    
    }
}

