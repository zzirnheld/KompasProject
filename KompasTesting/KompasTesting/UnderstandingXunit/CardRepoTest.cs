using Godot;
using Kompas;
using Kompas.Cards.Loading;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate;
using Moq;

namespace KompasTesting;

public class CardRepoTest
{
	public CardRepoTest()
	{
        var logger = new Mock<IKompasLogger>();
        Logger.Singleton.KompasLogger = logger.Object;
	}

    private class FileLoader : CardRepository.IFileLoader
    {
        public string? LoadFileAsText(string path)
        {
            string resourcesDir = Path.Combine("..","..","..","..","..", "Kompas");
            string newPath = path.Replace("res://", resourcesDir + "/");

            return File.ReadAllText(newPath);
        }

		public Texture2D? LoadSprite(string cardFileName) => null;
    }

    [Fact]
    public void ServerCard_Load_EachCardInCardsList_FinishesWithoutError()
    {
        var repo = new ServerCardRepository(new FileLoader(), true);

        var game = new Mock<IServerGame>();

        game.SetupGet(game => game.CardRepository)
            .Returns(repo);

		var player = new Mock<IPlayer>();
		player.SetupGet(p => p.Game)
			.Returns(game.Object);

		var stack = new Mock<IServerStackController>();
		game.SetupGet(g => g.StackController)
			.Returns(stack.Object);

        Assert.NotEmpty(CardRepository.CardNames);

        foreach (var cardName in CardRepository.CardNames)
        {
            var card = repo.InstantiateServerCard(cardName, game.Object, player.Object, 69, false);

            Assert.NotNull(card);
			
			//if (i++ > limit) break;
        }
    }
}