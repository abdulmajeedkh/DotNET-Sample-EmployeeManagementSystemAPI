using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Domain
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Adjust this path to point to your API project's appsettings.json
            var projectDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\EmployeeManagement.API"));
            var configPath = Path.Combine(projectDir, "appsettings.json");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(projectDir) // Set base path explicitly
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DbConnection"));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
