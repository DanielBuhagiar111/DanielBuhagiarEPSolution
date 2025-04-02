using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.DataContext
{
    public class PollDbContext : IdentityDbContext<CustomUser>
    {

        public PollDbContext(DbContextOptions<PollDbContext> options)
            : base(options)
        {
        }

        public DbSet<Poll> Polls { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
