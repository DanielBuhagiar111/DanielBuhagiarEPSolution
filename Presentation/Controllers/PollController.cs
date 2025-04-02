using DataAccess.DataContext;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Presentation.Factories;
using System.Linq;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {
        private readonly PollRepository _dbRepository;
        private readonly PollFileRepository _fileRepository;

        public PollController(PollRepositoryFactory repositoryFactory, IConfiguration configuration)
        {
            var repository = repositoryFactory.CreateRepository();

            if (repository is PollRepository dbRepository)
            {
                _dbRepository = dbRepository;
            }
            else if (repository is PollFileRepository fileRepository)
            {
                _fileRepository = fileRepository;
            }
        }

        public IActionResult Index()
        {
            if (_dbRepository != null)
            {
                var sortedPolls = _dbRepository.GetPolls().Cast<PollListDto>().OrderByDescending(p => p.DateCreated).ToList();
                return View(sortedPolls);
            }
            else
            {
                var sortedPolls = _fileRepository.GetPolls().Cast<PollListDto>().OrderByDescending(p => p.DateCreated).ToList();
                return View(sortedPolls);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        // Example of Method injection
        [HttpPost]
        public IActionResult Create(Poll poll, [FromServices] PollRepositoryFactory repositoryFactory)
        {
            var pollRepository = repositoryFactory.CreateRepository();

            if (pollRepository is PollRepository dbRepository)
            {
                dbRepository.CreatePoll(poll);
            }
            else if (pollRepository is PollFileRepository fileRepository)
            {
                fileRepository.CreatePoll(poll);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            if (_dbRepository != null)
            {
                var pollDetails = _dbRepository.GetPolls(id).Cast<PollDetailsDto>().FirstOrDefault();
                return View(pollDetails);
            }
            else
            {
                var pollDetails = _fileRepository.GetPolls(id).Cast<PollDetailsDto>().FirstOrDefault();
                return View(pollDetails);
            }
        }

        [HttpPost]
        public IActionResult Vote(int pollId, int chosenOption)
        {
            if (_dbRepository != null)
            {
                _dbRepository.Vote(pollId, chosenOption);
            }
            else
            {
                _fileRepository.Vote(pollId, chosenOption);
            }

            return RedirectToAction("Index");
        }
    }
}