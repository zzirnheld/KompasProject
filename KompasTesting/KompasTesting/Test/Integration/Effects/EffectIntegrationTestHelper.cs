using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate;
using Kompas.Cards.Loading;
using Kompas.Gamestate.Players;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Models;
using Kompas.Server.Networking;
using Kompas.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Cards.Controllers;

namespace Kompas.Test.Integration.Effects;

public static class EffectIntegrationTestHelper
{
    /// <summary>
    /// Creates a ServerGame, for the given inputs
    /// </summary>
    /// <param name="fileLoader">Should be able to "provide" the json for a particular card, if it's relevant for testing</param>
    /// <param name="debugMode">Whether cards should be allowed to be played regardless of stuff. Should probably be true during setup, then false after</param>
    /// <param name="playerNetworkers">In case you want to watch what packets get sent out, and send responses accordingly (i.e. for card targeting).
    /// Since everything is fake, you can just immediately call the relevant server->client packet's <see cref="IServerOrderPacket.Execute(ServerGame, ServerPlayer)">Execute</see> method.</param>
    /// <returns>The fully set-up game, ready for you to execute order packets on (i.e. to simulate eff activations)</returns>
    public static IServerGame CreateGame(IFileLoader fileLoader, Func<bool> debugMode, IServerNetworker[] playerNetworkers)
    {
        var repo = new ServerCardRepository(fileLoader, true);

        return ServerGameController.CreateGame(repo, debugMode, playerNetworkers);
    }

    private class ServerGameController : IServerGameController
    {
        public ServerGame ServerGame { get; set; } = null!;
        public ServerCardRepository CardRepository { get; }

        public IReadOnlyCollection<IServerNetworker> Networkers { get; private set; } = null!;
        public IPlayerController[] PlayerControllers { get; private set; } = null!;
        public IBoardController BoardController { get; private set; } = null!;

        public IGame Game => ServerGame;

        private ServerGameController(ServerCardRepository cardRepository)
        {
            CardRepository = cardRepository;
        }

        public static ServerGame CreateGame(ServerCardRepository cardRepository, Func<bool> debugMode, IServerNetworker[] playerNetworkers)
        {
            var controller = new ServerGameController(cardRepository);
            controller.PlayerControllers = [new PlayerController(), new PlayerController()];
            controller.BoardController = new BoardController();

            var ret = ServerGame.Create(controller, cardRepository, debugMode);

            var players = ServerPlayer.Create(controller,
                (player, index) => playerNetworkers[index]);
            controller.Networkers = playerNetworkers;

            ret.SetPlayers(players);
            return ret;
        }
    }

    private class PlayerController : IPlayerController
    {
        public IHandController HandController { get; } = new HandController();
        public IDiscardController DiscardController { get; } = new DiscardController();
        public IDeckController DeckController { get; } = new DeckController();
        public IAnnihilationController AnnihilationController { get; } = new AnnihilationController();

        public IGameCardInfo Avatar { get; set; } = null!;
        public int Pips { get; set; }
        public int PipsNextTurn { get; set; }
    }

    private class HandController : IHandController
    {
        public Hand HandModel { get; set; } = null!;
        public void Refresh() { }
    }

    private class DiscardController : IDiscardController
    {
        public Discard DiscardModel { get; set; } = null!;
        public void Refresh() { }
    }

    private class DeckController : IDeckController
    {
        public Deck DeckModel { get; set; } = null!;
        public void Refresh() { }
    }

    private class AnnihilationController : IAnnihilationController
    {
        public Annihilation AnnihilationModel { get; set; } = null!;
        public void Refresh() { }
    }

    private class BoardController : IBoardController
    {
        public void Move(ICardController card, MovePath path) { }
        public void Play(ICardController card) { }
        public void Remove(ICardController card) { }
    }
}