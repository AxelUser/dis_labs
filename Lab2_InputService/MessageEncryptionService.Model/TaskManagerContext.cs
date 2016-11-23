namespace MessageEncryptionService.Model
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class TaskManagerContext : DbContext
    {
        public TaskManagerContext()
            : base("name=DisTaskManagerConnection")
        {

        }

        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<PlannedTask> PlannedTasks { get; set; }
    }
}