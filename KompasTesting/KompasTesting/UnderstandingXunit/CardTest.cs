using Kompas;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Moq;

namespace KompasTesting;

public class CardTest
{
    private const string CardFileName = "Peanutbutter";
    private const string CardName = "Peanut";
    private const string EffText = "jelly";
    private const int CardID = 69;
    
    private readonly Mock<ICardRepository> repo;
    private readonly Mock<IServerGame> game;


    public CardTest()
    {
        var logger = new Mock<IKompasLogger>();
        Logger.Singleton.KompasLogger = logger.Object;

        game = new Mock<IServerGame>();
        repo = new Mock<ICardRepository>();

        game.SetupGet(game => game.CardRepository)
            .Returns(repo.Object);
        repo.Setup(repo => repo.AddKeywordHints(It.IsAny<string>()))
            .Returns(string.Empty);
    }

    private (ServerGameCard, Mock<IPlayer>) CreateServerCard(string cardName, string effText, int id, string cardFileName)
    {
        var player = new Mock<IPlayer>();
        player.SetupGet(player => player.Game)
            .Returns(game.Object);

        return (CreateServerCard(cardName, effText, id, cardFileName, player.Object), player);
    }

    private ServerGameCard CreateServerCard(string cardName, string effText, int id, string cardFileName, IPlayer player)
    {
        SerializableCard card = new()
        {
            cardName = cardName,
            effText = effText,
        };
        repo.Setup(repo => repo.FileNameFor(It.IsAny<string>()))
            .Returns(cardFileName);

        var ctrl = new Mock<ICardController>();

        //TODO fake the file name

        var toTest = ServerGameCard.Create(card, id, player, game.Object, ctrl.Object, Enumerable.Empty<IServerEffect>().ToArray(), false);

        Assert.NotNull(toTest);
        return toTest;
    }

    [Fact]
    public void ServerCard_Create_ValidData_FinishesWithoutError()
    {
        var (card, _) = CreateServerCard(CardName, EffText, CardID, CardFileName);

        Assert.Equal(CardName, card.CardName);
        Assert.Equal(EffText, card.EffText);
        Assert.Equal(CardFileName, card.FileName);
        Assert.Equal(CardID, card.ID);
    }

    [Fact]
    public void ServerCard_Create_MissingName_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => CreateServerCard(null, EffText, CardID, CardFileName));
    }

    [Fact]
    public void ServerCard_Create_MissingEffText_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => CreateServerCard(CardName, null, CardID, CardFileName));
    }

    [Fact]
    public void ServerCard_Remove_FromNowhere_FinishesWithoutError()
    {
        var (card, _) = CreateServerCard(CardName, EffText, CardID, CardFileName);

        var board = new Mock<IBoard>();
        game.SetupGet(game => game.Board)
            .Returns(board.Object);

        var stack = new Mock<IServerStackController>();
        game.SetupGet(g => g.StackController)
            .Returns(stack.Object);

        card.Remove();

        stack.Verify(s =>
            s.TriggerForCondition(Trigger.Remove, It.Is<TriggeringEventContext>(ctxt => IsCorrectRemoveContext(ctxt, card))));
    }

    private static bool IsCorrectRemoveContext(TriggeringEventContext ctxt, GameCard mainCard)
    {
        Assert.NotNull(ctxt.MainCardInfoBefore);
        Assert.Equal(mainCard, ctxt.MainCardInfoBefore.Card);
        
        Assert.NotNull(ctxt.MainCardInfoAfter);
        Assert.Equal(mainCard, ctxt.MainCardInfoAfter.Card);

        return true;
    }
}