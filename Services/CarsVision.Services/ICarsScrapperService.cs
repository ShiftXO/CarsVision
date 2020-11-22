namespace CarsVision.Services
{
    using System.Threading.Tasks;

    public interface ICarsScrapperService
    {
        Task PopulateDb(int pagesCount);
    }
}
