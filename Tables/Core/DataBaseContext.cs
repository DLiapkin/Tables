using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Tables.MVVM.Model;

namespace Tables.Core
{
    /// <summary>
    /// The class acts as an intermediary between the database and the application
    /// </summary>
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
        }
    }
}
