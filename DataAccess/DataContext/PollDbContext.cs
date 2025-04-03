using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.DataContext
{
    public class PollDbContext : IdentityDbContext<CustomUser> 
    {
        public PollDbContext(DbContextOptions<PollDbContext> options)
            : base(options)
        {
        }

        public DbSet<Poll> Polls { get; set; }
        public DbSet<UserVote> UserVotes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 

            builder.Entity<UserVote>()
                .HasKey(uv => new { uv.UserId, uv.PollId });
        }
    }
}