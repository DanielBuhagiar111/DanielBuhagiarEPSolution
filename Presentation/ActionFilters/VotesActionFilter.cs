using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.DataContext;
using Microsoft.Extensions.Configuration;
using DataAccess.Repositories;
using Domain.Interfaces;

namespace Presentation.ActionFilters
{
    public class VotesActionFilter : ActionFilterAttribute
    {
        private readonly PollDbContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IConfiguration _configuration;

        public VotesActionFilter(PollDbContext context, UserManager<CustomUser> userManager, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var pollId = (int)context.ActionArguments["pollId"];
            var userId = _userManager.GetUserId(context.HttpContext.User);
            var repositoryType = _configuration["RepositoryType"];

            if (userId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (repositoryType == "Database")
            {
                if (_context.UserVotes.Any(uv => uv.UserId == userId && uv.PollId == pollId))
                {
                    var controller = context.Controller as Controller;
                    if (controller != null)
                    {
                        controller.TempData["ErrorMessage"] = "You already voted. Vote not Submitted";
                    }

                    context.Result = new RedirectToActionResult("Index", "Poll", null);
                    return;
                }
            }
            else if (repositoryType == "File")
            {
                var pollRepository = context.HttpContext.RequestServices.GetService(typeof(IPollRepository)) as PollFileRepository;
                if (pollRepository != null && pollRepository.UserHasVoted(userId, pollId))
                {
                    var controller = context.Controller as Controller;
                    if (controller != null)
                    {
                        controller.TempData["ErrorMessage"] = "You already voted. Vote not Submitted";
                    }

                    context.Result = new RedirectToActionResult("Index", "Poll", null);
                    return;
                }
            }
            else
            {
                var controller = context.Controller as Controller;
                if (controller != null)
                {
                    controller.TempData["ErrorMessage"] = "Invalid Repository Type Configuration";
                }

                context.Result = new RedirectToActionResult("Index", "Poll", null);
                return;
            }

            await next();
        }
    }
}