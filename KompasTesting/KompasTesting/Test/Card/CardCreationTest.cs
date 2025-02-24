using Kompas.Cards.Loading;
using Kompas.Server.Gamestate;
using Kompas.Test.Card;
using Moq;

namespace Kompas.Test;

public class CardCreationTest
{
    private const string CardFileName = "Peanutbutter";
    private const string CardName = "Peanut";
    private const string EffText = "jelly";
    private const int CardID = 69;
    
    private readonly Mock<ICardRepository> repo;
    private readonly Mock<IServerGame> game;


    public CardCreationTest()
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
    public void ServerCard_Create_ValidData_FinishesWithoutError()
    {
        var (card, _) = CardCreationTestHelper.CreateServerCard(repo, game, CardName, EffText, CardID, CardFileName);

        Assert.Equal(CardName, card.CardName);
        Assert.Equal(EffText, card.EffText);
        Assert.Equal(CardFileName, card.FileName);
        Assert.Equal(CardID, card.ID);
    }

    [Fact]
    public void ServerCard_Create_MissingName_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => CardCreationTestHelper.CreateServerCard(repo, game, null!, EffText, CardID, CardFileName));
    }

    [Fact]
    public void ServerCard_Create_MissingEffText_ThrowsException()
    {
        Assert.Throws<ArgumentNullException>(() => CardCreationTestHelper.CreateServerCard(repo, game, CardName, null!, CardID, CardFileName));
    }
}