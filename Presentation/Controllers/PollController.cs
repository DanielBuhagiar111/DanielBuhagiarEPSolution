using DataAccess.DataContext;
using DataAccess.Repositories;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        private readonly IPollRepository _pollRepository;
        private readonly UserManager<CustomUser> _userManager;
        private readonly PollDbContext _context;
        private readonly IConfiguration _configuration;

        public PollController(IPollRepository pollRepository, UserManager<CustomUser> userManager, PollDbContext context, IConfiguration configuration)
        {
            _pollRepository = pollRepository;
            _userManager = userManager;
            _context = context;
            _configuration = configuration;
        }

        [Authorize]
        public IActionResult Index()
        {
            var sortedPolls = _pollRepository.GetPolls().Cast<PollListDto>().OrderByDescending(p => p.DateCreated).ToList();
            return View(sortedPolls);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // Example of method injection
        [Authorize]
        [HttpPost]
        public IActionResult Create(Poll poll, [FromServices] IPollRepository pollRepository)
        {
            pollRepository.CreatePoll(poll);
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Details(int id)
        {
            var pollDetails = _pollRepository.GetPolls(id).Cast<PollDetailsDto>().FirstOrDefault();
            return View(pollDetails);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Vote(int pollId, int chosenOption)
        {
            var userId = _userManager.GetUserId(User);

            if (_configuration["RepositoryType"] == "Database")
            {
                if (!_context.UserVotes.Any(uv => uv.UserId == userId && uv.PollId == pollId))
                {
                    _pollRepository.Vote(pollId, chosenOption);
                    _context.UserVotes.Add(new UserVote { UserId = userId, PollId = pollId });
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Vote Submited";
                }
                else
                {
                    TempData["ErrorMessage"] = "You have already voted for this poll. Vote not Submited";
                }
            }
            else
            {
                var fileRepo = (PollFileRepository)_pollRepository; 

                if (!fileRepo.UserHasVoted(userId, pollId))
                {
                    _pollRepository.Vote(pollId, chosenOption);
                    fileRepo.AddUserVote(userId, pollId);
                    TempData["SuccessMessage"] = "Vote Submited";
                }
                else
                {
                    TempData["ErrorMessage"] = "You have already voted for this poll. Vote not Submited";
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult Results()
        {
            var sortedPolls = _pollRepository.GetPolls(-1).Cast<Poll>().OrderByDescending(p => p.DateCreated).ToList();
            return View(sortedPolls);
        }
    }
}