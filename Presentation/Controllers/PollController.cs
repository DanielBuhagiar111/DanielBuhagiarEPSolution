using DataAccess.DataContext;
using DataAccess.Repositories;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Presentation.Controllers
{
    public class PollController : Controller
    {

        private readonly PollRepository _pollRepository;

        public PollController(PollRepository pollRepository)
        {
            _pollRepository = pollRepository;
        }

        public IActionResult Index()
        {
            var sortedPolls = _pollRepository.GetPolls().Cast<PollListDto>().OrderByDescending(p => p.DateCreated).ToList();

            return View(sortedPolls);
        }

        public IActionResult Create()
        {
            return View();
        }

        // Example of Method injection
        [HttpPost]
        public IActionResult Create(Poll poll, [FromServices] PollRepository pollRepository)
        {
            pollRepository.CreatePoll(poll);
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var pollDetails = _pollRepository.GetPolls(id)
                .Cast<PollDetailsDto>()
                .FirstOrDefault();

            if (pollDetails == null)
            {
                return NotFound();
            }

            return View(pollDetails);
        }

        [HttpPost]
        public IActionResult Vote(int pollId, int chosenOption)
        {
            _pollRepository.Vote(pollId, chosenOption);
            return RedirectToAction("Index"); 
        }
    }
}