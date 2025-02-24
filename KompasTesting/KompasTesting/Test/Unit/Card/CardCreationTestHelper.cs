using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Gamestate.Players;
using Kompas.Networking;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Moq;

namespace Kompas.Test.Unit.Card;

public static class CardCreationTestHelper {

    public static (ServerGameCard, IPlayer) CreateServerCard(Mock<ICardRepository> repo, Mock<IServerGame> game, string cardName, string effText, int id, string cardFileName)
    {
        var player = new Mock<IPlayer>();
        player.SetupGet(player => player.Game)
            .Returns(game.Object);

        var networker = new Mock<INetworker>();
        player.SetupGet(p => p.Networker)
            .Returns(networker.Object);
        player.SetupGet(p => p.Enemy)
            .Returns(player.Object); //Note: must do setups in the same function as creates the mock.

        return (CreateServerCard(repo, game, cardName, effText, id, cardFileName, player.Object), player.Object);
    }

    public static ServerGameCard CreateServerCard(Mock<ICardRepository> repo, Mock<IServerGame> game, string cardName, string effText, int id, string cardFileName, IPlayer player)
    {
        SerializableCard card = new()
        {
            cardName = cardName,
            effText = effText,
        };
        repo.Setup(r => r.FileNameFor(It.IsAny<string>()))
            .Returns(cardFileName);

        var ctrl = new Mock<ICardController>();

        //TODO fake the file name

        var toTest = ServerGameCard.Create(card, id, player, game.Object, ctrl.Object, Enumerable.Empty<IServerEffect>().ToArray(), false);

        Assert.NotNull(toTest);
        return toTest;
    }
}