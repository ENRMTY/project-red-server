using Microsoft.EntityFrameworkCore;
using ProjectRed.Core.Entities;

namespace ProjectRed.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserAuth> UserAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<UserAuth>()
                .HasIndex(a => new { a.Provider, a.ProviderUserId })
                .IsUnique();

            modelBuilder.Entity<UserAuth>()
                .HasIndex(a => a.NormalizedEmail)
                .IsUnique()
                .HasFilter("\"Provider\" = 'local'");

            modelBuilder.Entity<UserAuth>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.AuthMethods)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserSession>()
                .HasIndex(s => s.ExpiresAt);

            modelBuilder.Entity<UserSession>()
                .HasIndex(s => s.UserId);

            modelBuilder.Entity<UserSession>()
                .HasIndex(s => s.RefreshTokenHash)
                .IsUnique();

            modelBuilder.Entity<UserSession>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFavoritePlayer>()
                .HasKey(f => new { f.UserId, f.PlayerId });

            modelBuilder.Entity<UserFavoritePlayer>()
                .HasOne(f => f.User)
                .WithMany(u => u.FavoritePlayers)
                .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<UserFavoritePlayer>()
                .HasOne(f => f.Player)
                .WithMany(p => p.Fans)
                .HasForeignKey(f => f.PlayerId);

            modelBuilder.Entity<Club>()
                .HasIndex(c => c.Slug)
                .IsUnique();

            modelBuilder.Entity<Club>()
                .Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<Club>()
                .Property(c => c.ShortName)
                .HasMaxLength(5)
                .IsRequired();

            modelBuilder.Entity<PlayerCareer>()
                .HasIndex(c => new { c.PlayerId, c.ClubId, c.StartDate })
                .IsUnique();

            modelBuilder.Entity<PlayerCareer>()
                .HasOne(c => c.Player)
                .WithMany(p => p.CareerHistory)
                .HasForeignKey(c => c.PlayerId);

            modelBuilder.Entity<PlayerCareer>()
                .HasOne(c => c.Club)
                .WithMany(cl => cl.PlayersHistory)
                .HasForeignKey(c => c.ClubId);

            modelBuilder.Entity<PlayerCareer>()
                .Property(p => p.TransferType)
                .HasConversion<string>()
                .HasMaxLength(30);

            modelBuilder.Entity<Contract>()
                .HasOne(c => c.PlayerCareer)
                .WithMany(pc => pc.Contracts)
                .HasForeignKey(c => c.PlayerCareerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contract>()
                .HasIndex(c => new { c.PlayerCareerId, c.StartDate })
                .IsUnique();
        }
    }
}
