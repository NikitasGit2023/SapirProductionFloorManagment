using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SapirProductionFloorManagment.Server.BackgroundTasks;
using SapirProductionFloorManagment.Shared;


namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ConfigurationController : Controller
    {
        private readonly ILogger<ConfigurationController> _logger;
        public ConfigurationController(ILogger<ConfigurationController> logger) : base()
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Line> GetLinesData()
        {
            using var dbcon = new MainDbContext();
            try
            {
                var lines = dbcon.Lines.ToList();
                return lines;

            }catch (Exception ex)
            {
                _logger.LogError("GetLinesData: {ex.Message}", ex.Message);
               

            }
            return new List<Line>();
        }

        [HttpPost]
        public string PostNewLine(Line line)
        {
            try
            {
                using var dbcon = new MainDbContext();

                var existingLine = dbcon.Lines.FirstOrDefault(l => l.Name == line.Name);

                if (existingLine != null)
                {
                 
                    return "לא ניתן להוסיף קו שכבר קיים במערכת";
                }

                dbcon.Lines.Add(line);
                dbcon.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("PostNewLine: {ex.Message}", ex.Message);
                return ex.Message;
            }

            return "המידע הוסף בהצלחה";
        }


        [HttpPost]
        public string UpdateLine(Line line)
        {
            try
            {
                using var dbcon = new MainDbContext();

           
                var existingLine = dbcon.Lines.FirstOrDefault(l => l.Name == line.Name && l.LineId != line.LineId);

                if (existingLine != null)
                {               
                    return "לא ניתן לעדכן לקו שכבר קיים במערכת";
                }

                dbcon.Update(line);
                dbcon.SaveChanges();

                return "המידע עודכן בהצלחה";
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateLine: {ex.Message}", ex.Message);
                return ex.Message;
            }
        }


        [HttpPost]
        public string RemoveLine(Line line)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Lines.Remove(line);
                dbcon.SaveChanges();
                return "המידע הוסר בהצלחה";

            }
            catch(Exception ex) 
            {
                _logger.LogError("RemoveLine: {ex.Message}", ex.Message);
                return ex.Message;      
            } 
        }


        [HttpGet]
        public List<User> GetUsersData()
        {

            try
            {
                using var dbcon = new MainDbContext();
                var users = dbcon.Users.ToList();
                dbcon.SaveChanges();
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetUsersData: {ex.Message}", ex.Message);
            }
            return new List<User>();
        }


        [HttpPost]
        public string PostNewUser(User user)
        {
            try
            {
                using var dbcon = new MainDbContext();

                var existingUser = dbcon.Users.FirstOrDefault(u => u.FullName == user.FullName || u.Password == user.Password);

                if (existingUser != null)
                {          
                    return "שם משתמש או סיסמא תפוסים, אנא בחר שם משתמש וסיסמא ייחודיים";
                }

                dbcon.Users.Add(user);
                dbcon.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError("PostNewUser: {ex.Message}", ex.Message);
                return ex.Message;
            }

            return "המידע הוסף בהצלחה";
        }


        [HttpPost]
        public string UpdateUser(User user)
        {
            try
            {
                using var dbcon = new MainDbContext();


                var existingUser = dbcon.Users.FirstOrDefault(u => (u.FullName == user.FullName || u.Password == user.Password) && u.UserId != user.UserId);

                if (existingUser != null)
                {
                    return "לא ניתן לעדכן משתמש שכבר קיים במערכת";
                }

                dbcon.Update(user);
                dbcon.SaveChanges();
                return "המידע עודכן בהצלחה";
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateUser: {ex.Message}", ex.Message);
                return ex.Message;
            }
        }


        [HttpPost]
        public string RemoveUser(User user)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Remove(user);
                dbcon.SaveChanges();
                return "המידע הוסר בהצלחה";

            }
            catch (Exception ex)
            {
                _logger.LogError("RemoveUser: {ex.Message}", ex.Message);
                return ex.Message;
            }
        }

 
        [HttpGet]
        public List<Product> GetProductsData()
        {

            try
            {
                using var dbcon = new MainDbContext();
                var products = dbcon.Products.ToList();
                dbcon.SaveChanges();
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetProductsData: {ex.Message}", ex.Message); 
            }
            return new List<Product>();
        }



        [HttpGet]
        public List<LineWorkHours> GetLinesWorkHours()
        {

            try
            {
                using var dbcon = new MainDbContext();
                var lines = dbcon.LinesWorkHours.ToList();
                dbcon.SaveChanges();

                return lines;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetLinesData: {ex.Message}", ex.Message);
            }
            return new List<LineWorkHours>();
        }


        [HttpPost]
        public string PostLineWorkHours(LineWorkHours workHours)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.LinesWorkHours.Add(workHours);
                dbcon.SaveChanges();

            }
            catch (Exception ex)
            {
                _logger.LogError("PostNewLine: {ex.Message}", ex.Message);
                return ex.Message;
            }

            return "המידע הוסף בהצלחה";
        }



        [HttpPost]
        public string UpdateLineWorkHours(LineWorkHours workHours)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.LinesWorkHours.Update(workHours);

                var productionTimeScheduler = new ProductionTimeScheduler(_logger);
                var workOrder = dbcon.WorkOrdersFromXL.Where(e => e.OptionalLine1 == workHours.ReferencedToLine || 
                                                            e.OptionalLine2 == workHours.ReferencedToLine).FirstOrDefault(); 
                //if (workOrder != null)
                //{
                //    productionTimeScheduler.RegenerateWorkPlans(workOrder);
                //    dbcon.SaveChanges();

                //}

                return "המידע עודכן בהצלחה";
            }
            catch (Exception ex)
            {
                _logger.LogError("UpdateLine: {ex.Message}", ex.Message);
                return ex.Message;
            }
        }



        [HttpPost]
        public string RemoveLineWorkHours(LineWorkHours workHours)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.LinesWorkHours.Remove(workHours);
                dbcon.SaveChanges();
                return "המידע הוסר בהצלחה";

            }
            catch (Exception ex)
            {
                _logger.LogError("RemoveLine: {ex.Message}", ex.Message);
                return ex.Message;
            }
        }

        
    }
}
