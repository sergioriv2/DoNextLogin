using Microsoft.EntityFrameworkCore;
using ServerlessLogin.Models;

namespace ServerlessLogin.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<ValidationCode> ValidationCodes { get; set; }

        public DbSet<CodeType> CodeTypes { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<ValidationCode>()
            //    .HasKey(e => new { e.UserId });

            //modelBuilder.Entity<ValidationCode>()
            //    .HasOne(e => e.User)
            //    .WithMany(e => e.ValidationCodes)
            //    .HasForeignKey(e => e.UserId)
            //    .HasPrincipalKey(e => e.Id);

            //modelBuilder.Entity<RefreshToken>()
            //.HasKey(e => new { e.UserId });

            //modelBuilder.Entity<RefreshToken>()
            //    .HasOne(e => e.User)
            //    .WithMany(e => e.RefrehsTokens)
            //    .HasForeignKey(e => e.UserId)
            //    .HasPrincipalKey(e => e.Id);
        }
    }
}
