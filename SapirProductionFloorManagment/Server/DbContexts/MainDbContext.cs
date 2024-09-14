

using Microsoft.EntityFrameworkCore;
using SapirProductionFloorManagment.Shared;
using System.Diagnostics.Contracts;
using System.Data.SqlClient;

namespace SapirProductionFloorManagment.Server
{
    public class MainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Product> Products { get; set; }        
        public DbSet<WorkOrdersTableContext> WorkOrdersFromXL { get; set; }    
        public DbSet<LinesScheduleTableContext> LinesWorkSchedule { get; set; }
        public string _ConnectionString { get; }

        public MainDbContext()
        {
         
            var ConnectionString = @"Data Source=DESKTOP-10CMOF7\SQLEXPRESS;Initial Catalog=SapirProductsManagment6;User ID=account;Password=3194murkin;Encrypt=False";
            _ConnectionString = ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_ConnectionString);
            
        }








    }
}
