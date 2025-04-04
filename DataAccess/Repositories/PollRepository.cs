using DataAccess.DataContext;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Linq;

namespace DataAccess.Repositories
{
    public class PollRepository : IPollRepository 
    {
        private readonly PollDbContext _pollContext;

        // example of constructor injection
        public PollRepository(PollDbContext pollContext)
        {
            _pollContext = pollContext;
        }

        public void CreatePoll(Poll poll)
        {
            poll.DateCreated = DateTime.Now;
            _pollContext.Polls.Add(poll);
            _pollContext.SaveChanges();
        }

        public IQueryable<object> GetPolls(int? id = null)
        {
            if (id == null)
            {
                return _pollContext.Polls
                    .Select(p => new PollListDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        DateCreated = p.DateCreated
                    });
            }
            else if (id == -1) // Results
            {
                return _pollContext.Polls;
            }
            else // Details
            {
                return _pollContext.Polls
                    .Where(p => p.Id == id)
                    .Select(p => new PollDetailsDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Option1Text = p.Option1Text,
                        Option2Text = p.Option2Text,
                        Option3Text = p.Option3Text,
                        DateCreated = p.DateCreated
                    });
            }
        }

        public void Vote(int pollId, int chosenOption)
        {
            var poll = _pollContext.Polls.Find(pollId);

            if (poll != null)
            {
                switch (chosenOption)
                {
                    case 1:
                        poll.Option1VotesCount++;
                        break;
                    case 2:
                        poll.Option2VotesCount++;
                        break;
                    case 3:
                        poll.Option3VotesCount++;
                        break;
                    default:
                        throw new ArgumentException("Invalid option number.");
                }

                _pollContext.SaveChanges();
            }
        }
    }
}