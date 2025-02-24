using Kompas.Server.Cards.Loading;
using Kompas.Server.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Server.Networking;
using Kompas.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Models;
using Kompas.Cards.Controllers;

namespace Kompas.Test.Integration.Effects;

public class NoOpServerGameController : IServerGameController
{
	public ServerGame ServerGame { get; set; } = null!;
	public ServerCardRepository CardRepository { get; }

	public IReadOnlyCollection<IServerNetworker> Networkers { get; private set; } = null!;
	public IPlayerController[] PlayerControllers { get; private set; } = null!;
	public IBoardController BoardController { get; private set; } = null!;

	public IGame Game => ServerGame;

	private NoOpServerGameController(ServerCardRepository cardRepository)
	{
		CardRepository = cardRepository;
	}

	public static ServerGame CreateGame(ServerCardRepository cardRepository, Func<bool> debugMode, IServerNetworker[] playerNetworkers)
	{
		var controller = new NoOpServerGameController(cardRepository);
		controller.PlayerControllers = [new NoOpPlayerController(), new NoOpPlayerController()];
		controller.BoardController = new NoOpBoardController();

		var ret = ServerGame.Create(controller, cardRepository, debugMode);

		var players = ServerPlayer.Create(controller,
			(player, index) => playerNetworkers[index]);
		controller.Networkers = playerNetworkers;

		ret.SetPlayers(players);
		return ret;
	}

	private class NoOpPlayerController : IPlayerController
	{
		public IHandController HandController { get; } = new NoOpHandController();
		public IDiscardController DiscardController { get; } = new NoOpDiscardController();
		public IDeckController DeckController { get; } = new NoOpDeckController();
		public IAnnihilationController AnnihilationController { get; } = new NoOpAnnihilationController();

		public IGameCardInfo Avatar { get; set; } = null!;
		public int Pips { get; set; }
		public int PipsNextTurn { get; set; }
	}

	private class NoOpHandController : IHandController
	{
		public Hand HandModel { get; set; } = null!;
		public void Refresh() { }
	}

	private class NoOpDiscardController : IDiscardController
	{
		public Discard DiscardModel { get; set; } = null!;
		public void Refresh() { }
	}

	private class NoOpDeckController : IDeckController
	{
		public Deck DeckModel { get; set; } = null!;
		public void Refresh() { }
	}

	private class NoOpAnnihilationController : IAnnihilationController
	{
		public Annihilation AnnihilationModel { get; set; } = null!;
		public void Refresh() { }
	}

	private class NoOpBoardController : IBoardController
	{
		public void Move(ICardController card, MovePath path) { }
		public void Play(ICardController card) { }
		public void Remove(ICardController card) { }
	}
}