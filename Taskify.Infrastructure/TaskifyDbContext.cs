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

        public DbSet<TodoTask> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _appConfig.GetConnectionString("TaskifyConnectionString");

                optionsBuilder.UseSqlite(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        public override int SaveChanges()
        {
            var timestamp = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is BaseEntity baseEntity)
                {
                    switch(entry.State)
                    {
                        case EntityState.Added:
                            baseEntity.CreateDate = timestamp;
                            baseEntity.IsDeleted = false;
                            break;
                        case EntityState.Modified:
                            var isDeletedTag = entry.Property(nameof(baseEntity.IsDeleted));
                            
                            if (isDeletedTag.IsModified && isDeletedTag.CurrentValue is bool isDeleted && isDeleted )
                            {
                                baseEntity.DeletedDate = timestamp;
                                entry.Property(nameof(baseEntity.UpdateDate)).IsModified = false;
                            }
                            else
                            {
                                baseEntity.UpdateDate = timestamp;
                                entry.Property(nameof(baseEntity.DeletedDate)).IsModified = false;
                                entry.Property(nameof(baseEntity.IsDeleted)).IsModified = false;
                            }

                            entry.Property(nameof(baseEntity.CreateDate)).IsModified = false;
                            break;

                    }
                }
            }

            return base.SaveChanges();  
        }

        
    }
}
