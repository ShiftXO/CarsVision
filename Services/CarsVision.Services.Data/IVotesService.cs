namespace CarsVision.Services.Data
{
    using System.Threading.Tasks;

    public interface IVotesService
    {
        Task SetVoteAsync(string dealershipId, string userId, byte value);

        double GetAverageVotes(string dealershipId);
    }
}
