using DataAccess.DataContext;
using DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Presentation.Factories
{
    public class PollRepositoryFactory
    {
        private readonly IConfiguration _configuration;
        private readonly PollDbContext _dbContext;

        public PollRepositoryFactory(IConfiguration configuration, PollDbContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
        }

        public object CreateRepository()
        {
            var repositoryType = _configuration["RepositoryType"];
            if (repositoryType == "File")
            {
                return new PollFileRepository(Path.Combine(Directory.GetCurrentDirectory(), "Data", "polls.json"));
            }
            else
            {
                return new PollRepository(_dbContext);
            }
        }
    }
}