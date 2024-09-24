using Microsoft.Extensions.Logging;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.BackgroundTasks
{
    public class TimeSchedulerHelper
    {
        public ILogger? Logger { get; set; }

        public Dictionary<string, TimeSpan> BuildBreakDictionary(List<LineWorkHours> linesWorkHours)
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

        public DateTime AdjustForBreaks(DateTime workStart, TimeSpan workDuration, LineWorkHours workHours, Dictionary<string, TimeSpan> breaks)
        {
            DateTime workEnd = DateTime.MaxValue;

            workEnd = workStart.Add(workDuration);


            DateTime breakStart = string.IsNullOrEmpty(workHours.BreakStart) ? DateTime.MaxValue : DateTime.Parse(workHours.BreakStart);
            DateTime breakEnd = string.IsNullOrEmpty(workHours.BreakEnd) ? DateTime.MaxValue : DateTime.Parse(workHours.BreakEnd);


            if (workStart < breakStart && workEnd > breakStart)
            {
                TimeSpan breakDuration = breakEnd - breakStart;
                workEnd = workStart.Add(workDuration) + breakDuration;
            }

            return workEnd;
        }

        public double SetWorkDuration(int quantityInKg, LineWorkPlan lineSchedule)
        {
            var duration = 0.0;
            int relatedLine;

            if (int.TryParse(lineSchedule.RelatedToLine, out relatedLine))
            {
                if (relatedLine == 4 || relatedLine == 5)
                {

                    var prodRate = SetProductionRate(relatedLine, lineSchedule.SizeInMicron);

                    duration = (double)quantityInKg / prodRate / 24;
                    return duration;
                }
            }

            using var dbcon = new MainDbContext();

            var prodRateAnother = dbcon.Lines.Where(e => e.Name == lineSchedule.RelatedToLine)
                                              .Select(e => e.ProductionRate)
                                              .First();

            duration = (double)quantityInKg / prodRateAnother / 24;
            return duration;
        }

        public int SetProductionRate(int lineNum, int sizeInMicron)
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

        public int FindAvailableLine()
        {
            //TODO 

            return -1; 
        }

        public void RecheduleWorkPlan(LineWorkPlan workPlan)
        {
            using var dbcon = new MainDbContext();
            try
            {
                var converter = new DataConverter();

                workPlan.StartWork = null;
                workPlan.EndWork = null;
                workPlan.WorkDuraion = workPlan.LeftToFinish;
                workPlan.FormatedWorkDuration = converter.ConvertToTimeString(workPlan.WorkDuraion);
                //dbcon.RecheduledWorkPlans.Add(workPlan);
                dbcon.SaveChanges();


            }
            catch (Exception ex)
            {
                Logger.LogError("RecheduleWorkPlan: ex.Message", ex.Message);
            }




        }


    }
}
