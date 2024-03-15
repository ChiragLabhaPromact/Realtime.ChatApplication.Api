using Microsoft.AspNetCore.SignalR;
using Realtime.ChatApplication.DomianModels.Models.Messages;
using Realtime.ChatApplication.Service.Contracts.SignalR;
using StackExchange.Redis;
using System.Security.Claims;

namespace Realtime.ChatApplication.Api.Hubs
{
    public class ChatHub: Hub
    {
        private readonly IUserConnectionService _userConnectionService;


        public ChatHub(IUserConnectionService userConnectionService)
        {
            _userConnectionService = userConnectionService;
        }

        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;

            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _userConnectionService.AddConnectionAsync(userId, connectionId);

            await base.OnConnectedAsync();
        }

        public async Task SendMessage(Message message)
        {
            var receiverId = message.ReceiverId;
            var ConnectionId = await _userConnectionService.GetConnectionIdAsync(receiverId);
            message.SenderId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ConnectionId != null)
            {
                await Clients.Client(ConnectionId).SendAsync("ReceiveOne", message);
            }
        }
    }
}
