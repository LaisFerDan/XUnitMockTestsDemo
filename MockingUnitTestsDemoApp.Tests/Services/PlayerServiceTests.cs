using FluentAssertions;
using MockingUnitTestsDemoApp.Impl.Models;
using MockingUnitTestsDemoApp.Impl.Repositories.Interfaces;
using MockingUnitTestsDemoApp.Impl.Services;
using Moq;

namespace MockingUnitTestsDemoApp.Tests.Services
{
    public class PlayerServiceTests
    {
        private readonly PlayerService _subject;
        private readonly Mock<IPlayerRepository> _mockPlayerRepo;
        private readonly Mock<ITeamRepository> _mockTeamRepo;
        private readonly Mock<ILeagueRepository> _mockLeagueRepo;

        public PlayerServiceTests()
        {
            _mockPlayerRepo = new Mock<IPlayerRepository>();
            _mockTeamRepo = new Mock<ITeamRepository>();
            _mockLeagueRepo = new Mock<ILeagueRepository>();
            _subject = new PlayerService(_mockPlayerRepo.Object, _mockTeamRepo.Object, _mockLeagueRepo.Object);
        }

        [Fact]
        public void GetForLeague_HappyDay_RetornaTodosComoJogadores()
        {
            var id = 1;
            _mockLeagueRepo.Setup(mock => mock.IsValid(id)).Returns(true);
            _mockTeamRepo.Setup(mock => mock.GetForLeague(id)).Returns(GetFakeTeam());
            _mockPlayerRepo.Setup(mock => mock.GetForTeam(It.IsAny<int>())).Returns(GetFakePlayers());
            
            var players = _subject.GetForLeague(It.IsAny<int>());

            players.Should().AllBeAssignableTo<Player>().And.NotBeNull();
        }

        private List<Player> GetFakePlayers()
        {
            return new List<Player>
            {
                new Player {ID = 1, FirstName = "Menino", LastName = "Ney", 
                    DateOfBirth = DateTime.UtcNow, TeamID = 1},
                new Player {ID = 2, FirstName = "Alguma Coisa", LastName = "Messi", 
                    DateOfBirth = DateTime.UtcNow, TeamID = 2},
                new Player {ID = 3, FirstName = "Cristiano", LastName = "Ronaldo", 
                    DateOfBirth = DateTime.UtcNow, TeamID = 3}
            };
        }

        private List<Team> GetFakeTeam()
        {
            return new List<Team>
            {
                new Team {ID = 1, Name = "PSG", LeagueID = 1, FoundingDate = DateTime.UtcNow},
                new Team {ID = 2, Name = "MSC", LeagueID = 2, FoundingDate = DateTime.UtcNow},
                new Team {ID = 3, Name = "LIV", LeagueID = 3, FoundingDate = DateTime.UtcNow}
            };
        }
    }
}