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
        public void GetForLeague_HappyDay_ReturnPlayerListNotEmpty()
        {
            //Arrange
            _mockLeagueRepo.Setup(mock => mock.IsValid(It.IsAny<int>())).Returns(true);
            _mockTeamRepo.Setup(mock => mock.GetForLeague(It.IsAny<int>())).Returns(GetFakeTeams());
            _mockPlayerRepo.Setup(mock => mock.GetForTeam(It.IsAny<int>())).Returns(GetFakePlayers());
            
            //Act
            var playersResult = _subject.GetForLeague(It.IsAny<int>());

            //Assert
            playersResult.Should()
                .NotBeEmpty();

            VerifyLeagueIsValidTimeOnce();
            _mockLeagueRepo.Verify(x => x.IsValid(It.IsAny<int>()), Times.Once);
            _mockTeamRepo.Verify(x => x.GetForLeague(It.IsAny<int>()), Times.Once);
            _mockPlayerRepo.Verify(x => x.GetForTeam(It.IsAny<int>()), Times.Exactly(GetFakeTeams().Count));
        }

        [Fact]
        public void GetForLeague_InvalidLeagueId_EmptyPlayerList()
        {
            //Arrange
            _mockLeagueRepo.Setup(mock => mock.IsValid(It.IsAny<int>())).Returns(false);

            //Act
            var playersResult = _subject.GetForLeague(It.IsAny<int>());

            //Assert
            playersResult.Should()
                .BeEmpty();
            VerifyLeagueIsValidTimeOnce();
            _mockTeamRepo.Verify(x => x.GetForLeague(It.IsAny<int>()), Times.Never);
            _mockPlayerRepo.Verify(x => x.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void GetForLeague_LeagueWithoutTeams_EmptyListPlayers()
        {
            //Arrange
            _mockLeagueRepo.Setup(mock => mock.IsValid(It.IsAny<int>())).Returns(true);
            _mockTeamRepo.Setup(mock => mock.GetForLeague(It.IsAny<int>())).Returns(GetFakeTeams());

            //Act
            var playersResult = _subject.GetForLeague(It.IsAny<int>());

            //Assert
            playersResult.Should()
                .BeEmpty();
            VerifyLeagueIsValidTimeOnce();
            _mockTeamRepo.Verify(x => x.GetForLeague(It.IsAny<int>()), Times.Once);
            _mockPlayerRepo.Verify(x => x.GetForTeam(It.IsAny<int>()), Times.Never);
        }

        private List<Player> GetFakePlayers() 
            => new()
            {
                new Player {ID = 1, FirstName = "Menino", LastName = "Ney", 
                    DateOfBirth = DateTime.UtcNow, TeamID = 1},
                new Player {ID = 2, FirstName = "Alguma Coisa", LastName = "Messi", 
                    DateOfBirth = DateTime.UtcNow, TeamID = 2},
                new Player {ID = 3, FirstName = "Cristiano", LastName = "Ronaldo", 
                    DateOfBirth = DateTime.UtcNow, TeamID = 3}
            };

        private List<Team> GetFakeTeams() 
            => new ()
            {
                new Team {ID = 1, Name = "PSG", LeagueID = 1, FoundingDate = DateTime.UtcNow},
                new Team {ID = 2, Name = "MSC", LeagueID = 2, FoundingDate = DateTime.UtcNow},
                new Team {ID = 3, Name = "LIV", LeagueID = 3, FoundingDate = DateTime.UtcNow}
            };

        private void VerifyLeagueIsValidTimeOnce()
        {
            _mockLeagueRepo.Verify(x => x.IsValid(It.IsAny<int>()), Times.Once);
        }
    }
}