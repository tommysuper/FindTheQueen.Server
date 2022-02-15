using Microsoft.AspNetCore.SignalR;

namespace FindTheQueen.Server.Hubs
{
    public class EchoHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
