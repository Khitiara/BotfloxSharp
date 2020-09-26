using Microsoft.EntityFrameworkCore;

namespace Botflox.Bot.Data
{
    public class BotfloxDatabase : DbContext
    {
        public DbSet<GuildSettings> GuildsSettings { get; set; } = null!;
        public DbSet<UserSettings> UsersSettings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserSettings>()
                .HasIndex(u => u.MainCharacter)
                .IsUnique();
        }
        
        public BotfloxDatabase(DbContextOptions<BotfloxDatabase> options) : base(options)
        { }
    }
}