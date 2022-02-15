namespace FindTheQueen.Server
{
    public interface IGameRepository
    {
        Game Game { get; set; }
        List<Player> RegisterPlayers { get; }
    }

    public class GameRepository : IGameRepository
    {
        public GameRepository()
        {
            RegisterPlayers = new List<Player> { 
                new Player{UserName = "dannyboi", Password ="dre@margh_shelled"},
                new Player{UserName = "matty7", Password ="win&win99"}
            };
        }
        public List<Player> RegisterPlayers { get; private set; }

        public Game Game { get; set; }

    }
}
