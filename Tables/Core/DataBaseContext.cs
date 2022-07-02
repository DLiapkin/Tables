using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Tables.MVVM.Model;

namespace Tables.Core
{
    class DataBaseContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }

        public DataBaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TablesApp;Trusted_Connection=True;");
        }
    }
}
