using Microsoft.AspNetCore.SignalR;

namespace ProgramacionAvanzada.Books.Hubs
{
    public class BookCopiesStatsHub : Hub
    {
        public async Task SendStatsUpdate(int totalCopies, int lostCopies)
        {
            await Clients.All.SendAsync("ReceiveStatsUpdate", totalCopies, lostCopies);
        }
    }
}
