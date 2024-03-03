using Taskify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Taskify.Infrastructure
{
    public class TaskifyDbContext : DbContext
    {
        private readonly IConfiguration _appConfig;

        public TaskifyDbContext(IConfiguration appConfig)
        {
            _appConfig = appConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _appConfig.GetConnectionString("TaskifyConnectionString");

                optionsBuilder.UseSqlite(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<TodoTask> Tasks { get; set;}
    }
}
