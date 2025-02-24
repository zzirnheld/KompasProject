using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Server.Networking;
using Moq;

namespace Kompas.Test.Integration;

public class IntegrationTestSanityTests
{
	public IntegrationTestSanityTests()
	{
		var logger = new Mock<IKompasLogger>();
		Logger.Singleton.KompasLogger = logger.Object;
	}

	[Fact]
	public void FileLoader_FindsMagician()
	{
		var loader = new TestFileLoader();
		var magician = loader.LoadFileAsText(@"res://Jsons/Cards/tarocco/I - The Magician.json");
		Assert.False(string.IsNullOrEmpty(magician), magician);
	}

	[Fact]
	public void CreateAndSetupGame_PutsCardsInCorrectPlaces()
	{
		var networkers = new IServerNetworker[2] {
			new Mock<IServerNetworker>().Object, new Mock<IServerNetworker>().Object
		};
		var game = EffectIntegrationTestHelper.CreateAndSetupGame(new TestFileLoader(), () => false, networkers,
			new InitialCardPlacement() { CardName = "I - The Magician", ID = 0, IsAvatar = false, Location = Location.Board, Owner = 0, Position = (0, 0) });

		var magician = game.LookupCardByID(0);

		Assert.NotNull(magician);
		Assert.Equal("I - The Magician", magician.CardName);
		Assert.Equal(Location.Board, magician.Location);
		Assert.Equal(new Space(0, 0), magician.Position);
	}

}