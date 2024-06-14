﻿

using Microsoft.EntityFrameworkCore;
using SapirProductionFloorManagment.Shared;
using MySql.EntityFrameworkCore.Extensions;
using System.Diagnostics.Contracts;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;

namespace SapirProductionFloorManagment.Server
{
    public class ConfigurationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Product> Products { get; set; }

        public string _ConnectionString { get; }


        public ConfigurationDbContext()
        {
         
            var ConnectionString = @"Data Source=DESKTOP-10CMOF7\SQLEXPRESS;Initial Catalog=SapirProdMannagent;User ID=account;Password=3194murkin;Encrypt=False";
            _ConnectionString = ConnectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_ConnectionString);
            
        }








    }
}
