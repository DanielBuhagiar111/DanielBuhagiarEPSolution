using Domain.Interfaces;
using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataAccess.Repositories
{
    public class PollFileRepository : IPollRepository
    {
        private readonly string _filePath = "Data/Polls.json";
        private readonly string _userVotesFilePath = "Data/UserVotes.json";

        public PollFileRepository()
        {
        }

        public void CreatePoll(Poll poll)
        {
            List<Poll> polls = LoadPolls();

            if (polls.Count > 0)
            {
                poll.Id = polls.Max(p => p.Id) + 1;
            }
            else
            {
                poll.Id = 1;
            }

            poll.DateCreated = DateTime.Now;

            polls.Add(poll);

            SavePolls(polls);
        }

        public IQueryable<object> GetPolls(int? id = null)
        {
            List<Poll> polls = LoadPolls();

            if (id == null)
            {
                return polls.Select(p => new PollListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    DateCreated = p.DateCreated
                }).AsQueryable();
            }
            else if (id == -1) // Results
            {
                return polls.AsQueryable();
            }
            else // Details
            {
                return polls.Where(p => p.Id == id)
                    .Select(p => new PollDetailsDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        Option1Text = p.Option1Text,
                        Option2Text = p.Option2Text,
                        Option3Text = p.Option3Text,
                        DateCreated = p.DateCreated
                    }).AsQueryable();
            }
        }

        public void AddUserVote(string userId, int pollId)
        {
            var userVotes = LoadUserVotes();
            userVotes.Add(new UserVote { UserId = userId, PollId = pollId });
            SaveUserVotes(userVotes);
        }

        public bool UserHasVoted(string userId, int pollId)
        {
            var userVotes = LoadUserVotes();
            return userVotes.Any(uv => uv.UserId == userId && uv.PollId == pollId);
        }

        private List<UserVote> LoadUserVotes()
        {
            if (!File.Exists(_userVotesFilePath))
            {
                return new List<UserVote>();
            }
            string jsonData = File.ReadAllText(_userVotesFilePath);
            return JsonConvert.DeserializeObject<List<UserVote>>(jsonData) ?? new List<UserVote>();
        }

        private void SaveUserVotes(List<UserVote> userVotes)
        {
            string jsonData = JsonConvert.SerializeObject(userVotes, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_userVotesFilePath, jsonData);
        }


        public void Vote(int pollId, int chosenOption)
        {
            List<Poll> polls = LoadPolls();

            Poll poll = polls.FirstOrDefault(p => p.Id == pollId);

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

                SavePolls(polls);
            }
        }

        private List<Poll> LoadPolls()
        {
            if (!File.Exists(_filePath))
            {
                return new List<Poll>();
            }

            string jsonData = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<Poll>>(jsonData) ?? new List<Poll>();
        }

        private void SavePolls(List<Poll> polls)
        {
            string jsonData = JsonConvert.SerializeObject(polls, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(_filePath, jsonData);
        }
    }
}
