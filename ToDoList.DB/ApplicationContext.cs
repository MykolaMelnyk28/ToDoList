using Microsoft.EntityFrameworkCore;
using ToDoList.Shared.Entity;
using ToDoList.Shared.Helpers;

namespace ToDoList.DB
{
	public class ApplicationContext : DbContext
	{
		public DbSet<StateEntity> States { get; set; }
		public DbSet<PriorityEntity> Priorities { get; set; }
		public DbSet<UserEntity> Users { get; set; }
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
				x.Property(k => k.Name).HasMaxLength(16);
				x.HasData(PriorityEntityHelper.DefaultPriorities);
			});

			modelBuilder.Entity<StateEntity>(x =>
			{
				x.ToTable("States");
				x.HasKey(k => k.Id);
				x.Property(k => k.Name).HasMaxLength(16);
				x.HasData(StateEntityHelper.DefaultStates);
			});

			modelBuilder.Entity<TaskEntity>(x =>
			{
				x.ToTable("Tasks");
				x.HasKey(k => k.Id);
				x.Property(x => x.Name).HasMaxLength(255);
				x.HasIndex(x => x.Name).IsUnique();
				x.HasOne(x => x.Sate).WithMany(x => x.Tasks).HasForeignKey(f => f.StateId);
				x.HasOne(x => x.Priority).WithMany(x => x.Tasks).HasForeignKey(f => f.PriorityId);
				x.HasOne(x => x.User).WithMany(x => x.Tasks).HasForeignKey(f => f.UserId);
			});

			modelBuilder.Entity<UserEntity>(x =>
			{
				x.ToTable("Users");
				x.HasKey(k => k.Id);
				x.Property(x => x.FirstName).HasDefaultValue(string.Empty);
				x.Property(x => x.LastName).HasDefaultValue(string.Empty);
				x.Property(x => x.Phone).HasDefaultValue(string.Empty);
				x.HasIndex(x => x.Login).IsUnique();
				x.HasIndex(x => x.Email).IsUnique();
				x.HasIndex(x => x.Phone).IsUnique();
			});
		}
	}
}
