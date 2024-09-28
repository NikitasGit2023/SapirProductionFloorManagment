﻿using Microsoft.AspNetCore.Mvc;
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

        //[HttpGet]
        //public async Task<List<LineWorkPlan>> GetWorkPlans()
        //{
        //    try
        //    {
        //        using var dbcon = new MainDbContext();
        //        var schedule = dbcon.ActiveWorkPlans.Where(e => e.IsCalculted == false).ToList(); // bugged
        //        return await Task.FromResult(schedule);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger?.LogError("GetWorkPlans: {ex.Message}", ex.Message);

        //    }
        //    return new List<LineWorkPlan> { }; //check if null can be returned
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
