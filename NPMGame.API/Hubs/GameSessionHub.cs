using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace NPMGame.API.Hubs
{
    public class GameSessionHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}