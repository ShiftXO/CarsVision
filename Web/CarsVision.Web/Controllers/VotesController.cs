namespace CarsVision.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Votes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class VotesController : BaseController
    {
        private readonly IVotesService votesService;

        public VotesController(IVotesService votesService)
        {
            this.votesService = votesService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostVoteResponseModel>> Post(PostVoteInputModel inputModel)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            await this.votesService.SetVoteAsync(inputModel.DealershipId, userId, inputModel.Value);

            var averageVotes = this.votesService.GetAverageVotes(inputModel.DealershipId);

            return new PostVoteResponseModel { AverageVote = averageVotes };
        }
    }
}
