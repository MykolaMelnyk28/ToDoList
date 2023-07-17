using Microsoft.EntityFrameworkCore;
using ToDoList.Shared.Entity;
using ToDoList.Shared.Helpers;

namespace ToDoList.DB
{
	public class ApplicationContext : DbContext
	{
		public DbSet<PriorityEntity> Priorities { get; set; }
		public DbSet<TaskEntity> Tasks { get; set; }
		
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<PriorityEntity>(x =>
			{
				x.ToTable("Priorities");
				x.HasKey(k => k.Id);
				x.HasData(PriorityEntityHelper.DefaultPriorities);
			});

			modelBuilder.Entity<TaskEntity>(x =>
			{
				x.ToTable("Tasks");
				x.HasKey(k => k.Id);
				x.HasOne(x => x.Priority).WithMany(x => x.Tasks).HasForeignKey(f => f.PriorityId);
				x.HasIndex(x => x.Name).IsUnique();
			});
		}
	}
}
