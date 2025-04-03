using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.DataContext;

namespace Presentation.ActionFilters
{
    public class VotesActionFilter : ActionFilterAttribute
    {
        private readonly PollDbContext _context;
        private readonly UserManager<CustomUser> _userManager;

        public VotesActionFilter(PollDbContext context, UserManager<CustomUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var pollId = (int)context.ActionArguments["pollId"];
            var userId = _userManager.GetUserId(context.HttpContext.User);

            if (userId == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (_context.UserVotes.Any(uv => uv.UserId == userId && uv.PollId == pollId))
            {
                context.Result = new BadRequestObjectResult("You have already voted for this poll.");
                return;
            }

            await next();
        }
    }
}