using Bot.Common.Enums;
using Bot.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.Models.Data
{
	public class ApplicationContext:DbContext
	{
		public ApplicationContext()
		{
			Database.EnsureCreated();
        }
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Operation> Operations { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=localhost;Database=BotFinanceTracking;User Id=sa;Password=Valuetech@123;");
		}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Operation>()
                .HasOne(o => o.Category)
                .WithMany(c => c.Operations)
                .OnDelete(DeleteBehavior.Cascade);

            //modelBuilder.Entity<Category>()
            //    .HasOne(c => c.User)
            //    .WithMany(u => u.Categories)
            //    .OnDelete(DeleteBehavior.Cascade);
            
        }

    }
}

