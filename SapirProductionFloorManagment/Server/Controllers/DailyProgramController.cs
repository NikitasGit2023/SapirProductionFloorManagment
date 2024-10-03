using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapirProductionFloorManagment.Shared;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class DailyProgramController : Controller
    {

        private readonly ILogger<DailyProgramController> _logger;


        public DailyProgramController(ILogger<DailyProgramController> logger) : base()
        {
            _logger = logger;
        }

        [HttpGet]
        public Task<List<LineWorkPlan>> GetWorkPlans()
        {
            using var dbcon = new MainDbContext();
            var weeklyProgram = new List<LineWorkPlan>();  
            
            try
            {
                weeklyProgram = dbcon.ActiveWorkPlans.ToList();

            }
            catch (Exception ex) 
            {
                _logger.LogError("GetWorkPlans: {ex.Message}", ex.Message);
            }
            return  Task.FromResult(weeklyProgram);
          
        }

  
        [HttpGet]
        public Task<DateTime> GetLastWorkPlanCalculation()
        {
            DateTime lastCalc = DateTime.Now;
            try
            {
                using var dbcon = new MainDbContext();

                 lastCalc = dbcon.AppGeneralData
                                               .Select(a => a.LastWorkPlanCalculation)
                                               .FirstOrDefault();
                if (lastCalc == null)
                {
                    return Task.FromResult(DateTime.Now);
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError("GetLastWorkPlanCalculation: {ex.Message}", ex.Message);
            }
          

            return Task.FromResult(lastCalc);     
        }


        //[HttpPost]
        //public Task<string> SetLastWorkPlanCalculation(DateTime dateTime)
        //{
        //    using var dbcon = new MainDbContext();

        //}



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
    }
}
