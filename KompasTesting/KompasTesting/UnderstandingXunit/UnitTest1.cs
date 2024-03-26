using Kompas;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Cards;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Moq;

namespace KompasTesting;

public class ServerCardTest
{
    [Fact]
    public void ServerCard_Create_ValidData_FinishesWithoutError()
    {
        var logger = new Mock<IKompasLogger>();
        Logger.Singleton.KompasLogger = logger.Object;

        SerializableCard card = new()
        {
            cardName = "Peanut",
            effText = "jelly",
        };
        var player = new Mock<IPlayer>();
        var repo = new Mock<ICardRepository>();
        repo.Setup(repo => repo.FileNameFor(It.IsAny<string>()))
            .Returns("Peanutbutter");
        repo.Setup(repo => repo.AddKeywordHints(It.IsAny<string>()))
            .Returns(string.Empty);

        var game = new Mock<IServerGame>();
        game.SetupGet(game => game.CardRepository)
            .Returns(repo.Object);

        player.SetupGet(player => player.Game)
            .Returns(game.Object);
            
        var ctrl = new Mock<ICardController>();

        //TODO fake the file name

        var toTest = ServerGameCard.Create(card, 69, player.Object, game.Object, ctrl.Object, Enumerable.Empty<IServerEffect>().ToArray(), false);

        Assert.NotNull(toTest);
    }
}