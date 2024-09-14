using Microsoft.EntityFrameworkCore.Internal;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.BackgroundTasks
{
    public class ProductionCalculator
    {
        public ProductionCalculator() { }   

        public int CalculateProductionRate(int lineNum, int sizeInMicron)
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

        public double CalculateWorkDuration(int quantityInKg, int productionRate, LinesScheduleTableContext linesSchedule)
        {
            var duration = 0.0; 
            int relatedLine;

            if (int.TryParse(linesSchedule.RelatedToLine, out relatedLine))
            {
                if (relatedLine == 4 || relatedLine == 5)
                {
        
                    var prodRate = CalculateProductionRate(relatedLine, linesSchedule.SizeInMicron);
                
                    duration = (double)quantityInKg / prodRate / 24;  
                    return duration;
                }
            }
        
            duration = (double)quantityInKg / productionRate / 24; 
            return duration;
        }

        //not used
        public DateTime CalculateEndWork(DateTime startWork, double workDuration)
        {
          
            DateTime endWork = startWork.AddHours(workDuration);
        
            return endWork;
        }

        private async Task CalculateLinesSchdule()
        {
            var lineNum = 0;

            try
            {
                using var dbcon = new MainDbContext();
                var schdule = dbcon.LinesWorkSchedule.ToList();

                for( int i = 0; i<schdule.Count(); i++)
                {
                   
                    var schuduleItem = schdule[i];
                    int.TryParse(schuduleItem.RelatedToLine, out lineNum);
                    var productionRate = CalculateProductionRate(lineNum, schuduleItem.SizeInMicron);
                    var workDuration = CalculateWorkDuration(schuduleItem.QuantityInKg, productionRate, schuduleItem);
                    schuduleItem.WorkDuraion = workDuration;
                    dbcon.LinesWorkSchedule.Update(schuduleItem);
                    dbcon.SaveChanges();
                    
                }

            }catch(Exception ex) 
            {

            }    


        }
    }
}
