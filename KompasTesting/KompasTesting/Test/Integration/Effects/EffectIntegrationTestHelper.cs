using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate;
using Kompas.Cards.Loading;
using Kompas.Server.Networking;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Test.Integration.Effects;

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
    public static IServerGame CreateGame(IFileLoader fileLoader, Func<bool> debugMode, IServerNetworker[] playerNetworkers)
    {
        var repo = new ServerCardRepository(fileLoader, true);

        return NoOpServerGameController.CreateGame(repo, debugMode, playerNetworkers);
    }
}