namespace FindTheQueen.Server
{
    public class Game
    {
        public Game()
        {
            Player1 = new Player();
            Player2 = new Player();
        }

        public Player GetPlayer(string connectionId)
        {
            if (Player1 != null && Player1.ConnectionId == connectionId)
            {
                return Player1;
            }
            if (Player2 != null && Player2.ConnectionId == connectionId)
            {
                return Player2;
            }
            return null;
        }
        public bool HasPlayer(string connectionId)
        {
            if (Player1 != null && Player1.ConnectionId == connectionId)
            {
                return true;
            }
            if (Player2 != null && Player2.ConnectionId == connectionId)
            {
                return true;
            }
            return false;
        }

        public bool CheckQueenPosition(int guess)
        {
            if (QueenPosition == guess)
            {
                Spotter.Points++;
                return true;
            }
            else
            {
                Dealer.Points++;
                return false;
            }

        }

        public void NextRound()
        {
            if (Dealer == Player1)
            {
                Dealer = Player2;
                Spotter = Player1;
                Player1.IsDealer = false;
                Player2.IsDealer = true;

            }
            else
            {
                Dealer = Player1;
                Spotter = Player2;
                Player1.IsDealer = true;
                Player2.IsDealer = false;


            }
            Rounds++;
        }

        public Player CheckVictory()
        {
            if (Rounds == 5)
            {
                if (Player1.Points > Player2.Points)
                {
                    return Player1;
                }
                else
                {
                    return Player2;
                }
            }
            return null;
        }

       

        public int PlayerNumber { get; set; } = 0;
        public int Rounds { get; set; } = 1;
        public int QueenPosition { get; set; }
        // Properties
        public string Id { get; set; }
        public bool InProgress { get; set; }
        public Player Dealer { get;  set; }
        public Player Spotter { get;  set; }
        public Player CurrentPlayer { get; set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }

    }
}
