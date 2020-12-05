namespace CarsVision.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;

    public class VotesService : IVotesService
    {
        private readonly IRepository<Vote> votesRepository;

        public VotesService(IRepository<Vote> votesRepository)
        {
            this.votesRepository = votesRepository;
        }

        public double GetAverageVotes(string dealershipId)
        {
            return this.votesRepository.All()
                .Where(x => x.DealershipId == dealershipId)
                .Average(x => x.Value);
        }

        public async Task SetVoteAsync(string dealershipId, string userId, byte value)
        {
            var vote = this.votesRepository.All().FirstOrDefault(x => x.DealershipId == dealershipId && x.UserId == userId);

            if (vote == null)
            {
                vote = new Vote
                {
                    DealershipId = dealershipId,
                    UserId = userId,
                };

                await this.votesRepository.AddAsync(vote);
            }

            vote.Value = value;

            await this.votesRepository.SaveChangesAsync();
        }
    }
}
