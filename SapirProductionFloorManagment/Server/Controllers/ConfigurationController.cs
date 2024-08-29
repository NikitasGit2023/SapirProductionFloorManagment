using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SapirProductionFloorManagment.Shared;


namespace SapirProductionFloorManagment.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ConfigurationController : Controller
    {

        [HttpGet]
        public List<Line> GetLinesData()
        {
           
            try
            {
                using var dbcon = new MainDbContext();
                var lines =  dbcon.Lines.ToList();
                dbcon.SaveChanges();    
                return lines;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return new List<Line>();
        }

        
        [HttpPost]
        public string PostNewLine(Line line)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Lines.Add(line);
                dbcon.SaveChanges();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "הקו נוסף בהצלחה";
        }



        [HttpPost]
        public string UpdateLine(Line line)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Update(line);
                dbcon.SaveChanges();
                return "המידע עודכן בהצלחה";
            }
            catch (Exception ex)
            {
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
                return "הקו הוסר בהצלחה";

            }
            catch(Exception ex) 
            {
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
                Console.WriteLine(ex.Message);
            }
            return new List<User>();
        }

        [HttpPost]
        public string PostNewUser(User user)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Users.Add(user);
                dbcon.SaveChanges();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "הקו נוסף בהצלחה";
        }

        [HttpPost]
        public string UpdateUser(User user)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Update(user);
                dbcon.SaveChanges();
                return "המידע עודכן בהצלחה";

            }
            catch (Exception ex)
            {
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
                return "המידע עודכן בהצלחה";

            }
            catch (Exception ex)
            {
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
                Console.Write(ex.Message);
            }
            return new List<Product>();
        }

        [HttpPost]
        public string PostNewProduct(Product product)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Products.Add(product);
                dbcon.SaveChanges();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "המוצר נוסף בהצלחה";
        }

        [HttpPost]
        public string UpdateProduct(Product product)
        {
            try
            {
                using var dbcon = new MainDbContext();
                dbcon.Update(product);
                dbcon.SaveChanges();
                return "המידע עודכן בהצלחה";

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }





    }
}
