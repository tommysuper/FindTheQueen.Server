namespace FindTheQueen.Server
{
    public class Player
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Id { get; set; }
        public bool IsLoggedIn { get; set; }

        public string ConnectionId { get; set; }
        public int Points { get; set; }
        public bool IsDealer { get; set; }
    }
}
