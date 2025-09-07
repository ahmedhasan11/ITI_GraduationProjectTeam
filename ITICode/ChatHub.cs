using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public async Task SendMessage(string threadId, string senderId, string text)
    {
        await Clients.Group(threadId).SendAsync("ReceiveMessage", senderId, text);
    }

    public async Task JoinThread(string threadId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, threadId);
    }
}
