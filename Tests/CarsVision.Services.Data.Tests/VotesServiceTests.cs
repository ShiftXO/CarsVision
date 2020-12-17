namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Threading.Tasks;

    using CarsVision.Data;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class VotesServiceTests
    {
        private readonly VotesService service;
        private readonly ApplicationDbContext dbContext;
        private readonly IRepository<Vote> votesRepository;

        public VotesServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);

            this.votesRepository = new EfRepository<Vote>(this.dbContext);
            this.service = new VotesService(this.votesRepository);
        }

        [Fact]
        public async Task AverageVoteShouldReturnCorrect()
        {
            var vote = new Vote() { Id = 1, DealershipId = "id", Value = 2, };
            var vote1 = new Vote() { Id = 2, DealershipId = "id", Value = 3, };
            var vote2 = new Vote() { Id = 3, DealershipId = "id", Value = 5, };
            var vote3 = new Vote() { Id = 4, DealershipId = "id2", Value = 5, };

            this.dbContext.Add(vote);
            this.dbContext.Add(vote1);
            this.dbContext.Add(vote2);
            this.dbContext.Add(vote3);
            await this.dbContext.SaveChangesAsync();

            var result = this.service.GetAverageVotes("id");
            var result1 = this.service.GetAverageVotes("id2");

            Assert.Equal(3.3333333333333335, result);
            Assert.Equal(5, result1);
        }

        [Fact]
        public async Task SetVoteShouldSetCorrect()
        {
            await this.service.SetVoteAsync("dealershipId", "userId", 2);

            var count = this.votesRepository.All();
            var vote = await this.votesRepository.All().FirstAsync(x => x.DealershipId == "dealershipId");
            Assert.Equal(1, await count.CountAsync());
            Assert.Equal(2, vote.Value);
        }

        [Fact]
        public async Task SetVoteShouldChangeVoteCorrect()
        {
            await this.service.SetVoteAsync("dealershipId", "userId", 2);
            await this.service.SetVoteAsync("dealershipId", "userId", 5);

            var count = this.votesRepository.All();
            var vote = await this.votesRepository.All().FirstAsync(x => x.DealershipId == "dealershipId");

            Assert.Equal(1, await count.CountAsync());
            Assert.Equal(5, vote.Value);
        }

        [Fact]
        public async Task SetVoteShouldSetVoteValuesCorrect()
        {
            await this.service.SetVoteAsync("dealershipId", "userId", 2);

            var vote = await this.votesRepository.All().FirstAsync(x => x.DealershipId == "dealershipId");

            Assert.Equal("dealershipId", vote.DealershipId);
            Assert.Equal("userId", vote.UserId);
            Assert.Equal(2, vote.Value);
        }
    }
}
