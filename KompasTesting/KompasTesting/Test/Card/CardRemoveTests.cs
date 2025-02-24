using Kompas;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Locations.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate;
using Kompas.Test.Card;
using Moq;

namespace Kompas.Test;

public class CardRemoveTest
{
    private const string CardFileName = "Peanutbutter";
    private const string CardName = "Peanut";
    private const string EffText = "jelly";
    private const int CardID = 69;
    
    private readonly Mock<ICardRepository> repo;
    private readonly Mock<IServerGame> game;


    public CardRemoveTest()
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

    [Fact]
    public void ServerCard_Remove_FromNowhere_FinishesWithoutError()
    {
        var (card, _) = CardCreationTestHelper.CreateServerCard(repo, game, CardName, EffText, CardID, CardFileName);

        //Required for stashing adjacent cards
        var board = new Mock<IBoard>();
        game.SetupGet(game => game.Board)
            .Returns(board.Object);

        var stack = new Mock<IServerStackController>();
        game.SetupGet(g => g.StackController)
            .Returns(stack.Object);

        card.Remove();

        stack.Verify(s => s.TriggerFor(It.Is<IEventContext>(ctxt
            => IsCorrectRemoveContext(ctxt, Trigger.Remove, card, Location.Nowhere))));
    }

    [Fact]
    public void ServerCard_Remove_FromILocationModel_FinishesWithoutError()
    {
        var (card, player) = CardCreationTestHelper.CreateServerCard(repo, game, CardName, EffText, CardID, CardFileName);

        var locationModel = new Mock<ILocationModel>();
        locationModel.SetupGet(l => l.Location)
            .Returns(Location.Annihilation);
        card.LocationModel = locationModel.Object;

        //Required for stashing adjacent cards
        var board = new Mock<IBoard>();
        game.SetupGet(game => game.Board)
            .Returns(board.Object);

        var stack = new Mock<IServerStackController>();
        game.SetupGet(g => g.StackController)
            .Returns(stack.Object);

        card.Remove();

        stack.Verify(s => s.TriggerFor(It.Is<IEventContext>(ctxt
            => IsCorrectRemoveContext(ctxt, Trigger.Remove, card, locationModel.Object.Location))));

        locationModel.Verify(l
            => l.Remove(It.Is<GameCard>(c => c == card)));
    }

    private static bool IsCorrectRemoveContext(IEventContext ctxt, string condition, GameCard mainCard, Location locationBefore)
    {
        Assert.Equal(condition, ctxt.TriggeringEvent);

        Assert.NotNull(ctxt.MainCardBefore);
        Assert.Equal(mainCard, ctxt.MainCardBefore.Card);
        Assert.Equal(locationBefore, ctxt.MainCardBefore.Location);
        
        Assert.NotNull(ctxt.MainCardAfter);
        Assert.Equal(mainCard, ctxt.MainCardAfter.Card);

        return true;
    }
}