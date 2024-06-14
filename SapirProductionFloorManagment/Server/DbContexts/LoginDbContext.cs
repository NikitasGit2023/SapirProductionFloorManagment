using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SapirProductionFloorManagment.Shared;


namespace SapirProductionFloorManagment.Server.DbContexts

{
    public class LoginDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public string ConnectionString { get; set; }    

        public LoginDbContext()
        {

            var connectionString = @"server=localhost;port=3306;database=sapirprodmannagment;user=root;password=3194murkin";
            ConnectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConnectionString);

        }


    }

 


}
