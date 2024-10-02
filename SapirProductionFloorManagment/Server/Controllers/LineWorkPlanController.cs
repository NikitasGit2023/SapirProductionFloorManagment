using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SapirProductionFloorManagment.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class LineWorkPlanController : ControllerBase
    {
        private readonly ILogger<LineWorkPlanController> _logger;

        public LineWorkPlanController(ILogger<LineWorkPlanController> logger)
        {
            _logger = logger;
        }

        // Получение расписания работы линий
        [HttpGet]
        public async Task<List<LineWorkPlan>> GetWorkSchedule()
        {
            try
            {
                using (var dbcon = new MainDbContext())
                {
                    var workSchedule = await dbcon.ActiveWorkPlans.ToListAsync(); // Берем данные из ActiveWorkPlans
                    return workSchedule;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching work schedule: {Message}", ex.Message);
                return new List<LineWorkPlan>();  // если произошла ошибка, вернем пустой список
            }
        }

        // Получение списка всех линий
        [HttpGet]
        public async Task<List<Line>> GetLines()
        {
            try
            {
                using (var dbcon = new MainDbContext())
                {
                    var lines = await dbcon.Lines.ToListAsync();
                    return lines;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching lines data: {Message}", ex.Message);
                return new List<Line>();  // если произошла ошибка, вернем пустой список
            }
        }
    }
}
