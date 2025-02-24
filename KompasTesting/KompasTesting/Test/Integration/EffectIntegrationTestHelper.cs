using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate;
using Kompas.Cards.Loading;
using Kompas.Server.Networking;
using Kompas.Server.Gamestate.Players;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate;

namespace Kompas.Test.Integration;

public readonly struct InitialCardPlacement
{
    //Creation info
    public required string CardName { get; init; }
    public required int Owner { get; init; }
    public required int ID { get; init; }
    public required bool IsAvatar { get; init; } = false;

    //Initial spot
    public required Location Location { get; init; }

    public Space? Position { get; init; }
    public int? ControllerOverride { get; init; }
    public int Controller => ControllerOverride ?? Owner;

    //TODO: initial stats

    public InitialCardPlacement() { }
}

public static partial class EffectIntegrationTestHelper
{
    /// <summary>
    /// Creates a ServerGame, for the given inputs
    /// </summary>
    /// <param name="fileLoader">Should be able to "provide" the json for a particular card, if it's relevant for testing</param>
    /// <param name="debugMode">Whether cards should be allowed to be played regardless of stuff. Should probably be true during setup, then false after</param>
    /// <param name="playerNetworkers">In case you want to watch what packets get sent out, and send responses accordingly (i.e. for card targeting).
    /// Since everything is fake, you can just immediately call the relevant server->client packet's <see cref="IServerOrderPacket.Execute(ServerGame, ServerPlayer)">Execute</see> method.</param>
    /// <returns>The fully set-up game, ready for you to execute order packets on (i.e. to simulate eff activations)</returns>
    public static (IServerGame, ServerCardRepository) CreateGame(IFileLoader fileLoader, Func<bool> debugMode, IServerNetworker[] playerNetworkers)
    {
        var repo = new ServerCardRepository(fileLoader, true);

        return (NoOpServerGameController.CreateGame(repo, debugMode, playerNetworkers), repo);
    }

    private class BoolWrapper(Func<bool> fallback)
    {
        public bool? overrideBool = null;
        private readonly Func<bool> fallback = fallback;
        public bool Value => overrideBool ?? fallback();
    }

    public static IServerGame CreateAndSetupGame(IFileLoader fileLoader, Func<bool> debugMode, IServerNetworker[] playerNetworkers, params InitialCardPlacement[] initialCardPlacements)
    {
        var overridingDebugMode = new BoolWrapper(debugMode) { overrideBool = true };
        var (game, repo) = CreateGame(fileLoader, () => overridingDebugMode.Value, playerNetworkers);

        foreach (var placement in initialCardPlacements)
        {
            var card = repo.InstantiateServerCard(placement.CardName, game, game.Players[placement.Owner], placement.ID, placement.IsAvatar);

            switch (placement.Location)
            {
                case Location.Board:
                    var pos = placement.Position ?? throw new NullReferenceException($"{nameof(placement.Position)} not populated for {placement.CardName}");
                    game.Board.Play(card, pos, game.Players[placement.Controller], stackSrc: null);
                    break;
                case Location.Discard:
                    game.Players[placement.Controller].Discard.Add(card);
                    break;
                case Location.Hand:
                    game.Players[placement.Controller].Hand.Add(card);
                    break;
                case Location.Deck:
                    game.Players[placement.Controller].Deck.Add(card);
                    break;
                case Location.Annihilation:
                    game.Players[placement.Controller].Annihilation.Add(card);
                    break;
                default:
                    throw new InvalidOperationException($"Can't set up card {placement.CardName} in location {placement.Location}");
            }
        }

        return game;
    }
}