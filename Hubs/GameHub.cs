using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace FindTheQueen.Server.Hubs
{
    public interface IGameClient
    {
        Task Turn(string message, bool isTurn);
        Task Concede();
        Task Victory(string player);

        Task PlayerLogin(Player player);

        Task MessageReceived(string message);
        Task IsLoggedIn(string message);

        Task Dispose();

    }

    public class GameHub : Hub<IGameClient>
    {
        private IGameRepository _repository;
        private readonly Random _random;

        public GameHub(IGameRepository repository, Random random)
        {
            _repository = repository;
            _random = random;
        }

        public async Task TurnEnd(int position)
        {
            if (_repository.Game.CurrentPlayer.IsDealer)
            {
                _repository.Game.QueenPosition = position;
                await Clients.Client(_repository.Game.Dealer.ConnectionId)
               .Turn("Please wait for Spotter to Response",
               false);
                await Clients.Client(_repository.Game.Spotter.ConnectionId).
               Turn("Dealer has choosen the position number to put the Queen in, please make your guess(0-4)",
               true);
               SwitchPlayer(_repository.Game);
            }
            else
            {
                var result = _repository.Game.CheckQueenPosition(position);

                if (result)
                {
                    await Clients.Group(_repository.Game.Id).MessageReceived("Spooter has successfully spotted the Queen, Plus 1 point");
                }
                else
                {
                    await Clients.Group(_repository.Game.Id).MessageReceived("Spooter failed to spot the Queen, Plus 1 point to Dealer");
                }

                var winner = _repository.Game.CheckVictory();

                if (winner == null)
                {
                    _repository.Game.NextRound();
                    StartTurn(_repository.Game);
                }
                else
                {
                    

                    Player looser = winner == _repository.Game.Player1 ? 
                        _repository.Game.Player2: _repository.Game.Player1;
                    await Clients.Client(winner.ConnectionId)
                            .Turn(String.Format("You have won the Game with {0} points", winner.Points),
                            false);
                    await Clients.Client(looser.ConnectionId)
                            .Turn(String.Format("You are defeted but your oppnant, your score is {0} points", looser.Points),
                            false);
                    await Clients.Group(_repository.Game.Id).Dispose();
                }

            }

            
        }

        public async Task PlayerLogin(Player player)
        {
            if (player == null)
            {
                await Clients.Caller.MessageReceived("Please Login with valid user credential");
                return;
            }

            if (_repository.Game != null && _repository.Game.InProgress)
            {
                await Clients.Caller.MessageReceived("There are already 2 players playing the game, please retry later");
                return;
            }
            var existPlayer = _repository.RegisterPlayers.Where(rp => rp.UserName == player.UserName && rp.Password == player.Password).FirstOrDefault();
            if (existPlayer != null)
            {
                if (_repository.Game is null)
                {
                    var game = new Game { Id = Guid.NewGuid().ToString() };
                    game.Player1.ConnectionId = Context.ConnectionId;
                    _repository.Game = game;
                    await Clients.Caller.IsLoggedIn(String.Format("Welcome {0}, Please wait for other player to logon", player.UserName));
                }
                else
                {
                    _repository.Game.Player2.ConnectionId = Context.ConnectionId;
                    _repository.Game.InProgress = true;
                    await Clients.Caller.IsLoggedIn(String.Format("Welcome {0}, The game will begin soon", player.UserName));
                    AssignPlayers(_repository.Game);
                }
                await Groups.AddToGroupAsync(Context.ConnectionId, _repository.Game.Id);

            }
            else
            {
                await Clients.Caller.MessageReceived("Please Login with valid user credential");
            }

        }


        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.MessageReceived("Please Login to start playing");

            await base.OnConnectedAsync();
           
        }

        


        private void AssignPlayers(Game game)
        {
            var result = _random.Next(2);

            if (result == 1)
            {
                game.Dealer = game.Player1;
                game.Spotter = game.Player2;
                game.Player1.IsDealer = true;
                game.Player2.IsDealer = false;
                game.CurrentPlayer = game.Player1;
            }
            else
            {
                game.Dealer = game.Player2;
                game.Spotter = game.Player1;
                game.Player1.IsDealer = false;
                game.Player2.IsDealer = true;
                game.CurrentPlayer = game.Player2;
            }

            StartTurn(game);
            //await Clients.Group(game.Id).Turn(Game.RedCell);
        }


        private async void StartTurn(Game game)
        {
            await Clients.Client(game.Dealer.ConnectionId)
                .Turn("You are the DEALER now, please choose a position number from 0 to 4 when you want to put the Queen",
                true);
            await Clients.Client(game.Spotter.ConnectionId).
                Turn("You are the SPOTTER now, please wait for the DEALER to choose the position of the Queen",
                false);
        }

        private void SwitchPlayer(Game game)
        {
            if (game.CurrentPlayer == game.Player1)
            {
                game.CurrentPlayer = game.Player2;
            }
            else
            {
                game.CurrentPlayer = game.Player1;
            }
        }

    }
}
