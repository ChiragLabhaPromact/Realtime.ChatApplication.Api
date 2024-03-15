using Realtime.ChatApplication.DomianModels.Models.Dto.Messages;
using Realtime.ChatApplication.DomianModels.Models.Dto.Response;
using Realtime.ChatApplication.DomianModels.Models.Messages;
using Realtime.ChatApplication.Repository.Contracts.Messages;
using Realtime.ChatApplication.Repository.Implementations.Messages;
using Realtime.ChatApplication.Service.Contracts.Messages;
using Realtime.ChatApplication.Service.Implementations.Auth;

namespace Realtime.ChatApplication.Service.Implementations.Messages
{
    public class MessageService : IMessageService
    {
        private readonly JwtToken _jwtToken;
        private readonly IMessageRepository _messageRepository;

        public MessageService(JwtToken jwtToken, IMessageRepository messageRepository)
        {
            _jwtToken = jwtToken;
            _messageRepository = messageRepository;
        }
        public async Task<Response> SendMessage(SendMessage send)
        {
            var senderId = _jwtToken.GetAuthenticatedUserId();

            var message = new Message
            {
                Id = Guid.NewGuid().ToString(),
                SenderId = senderId,
                ReceiverId = send.receiverId,
                content = send.content,
                Timestemp = DateTime.UtcNow,

            };

            var result = await _messageRepository.SendMessage(message);

            return new Response(result);
        }

        public async Task<Response> EditMessage(string id, string content)
        {
            var message = await _messageRepository.GetMessageById(id);

            if(message == null) 
            {
                return new Response("Message Not Found");
            }

            var userId = _jwtToken.GetAuthenticatedUserId();

            if(userId != message.SenderId) 
            {
                return new Response("Unauthorized access");
            }

            message.content = content;

            await _messageRepository.UpdateMessage(message);

            return new Response(message);

        }
        public async Task<Response> DeleteMessage(string id)
        {
            var message = await _messageRepository.GetMessageById(id);

            if (message == null)
            {
                return new Response("Message Not Found");
            }

            var userId = _jwtToken.GetAuthenticatedUserId();

            if (userId != message.SenderId)
            {
                return new Response("Unauthorized access");
            }

            await _messageRepository.DeleteMessage(message);

            return new Response(new { Message = "Message Deleted Successfully" });
        }
        public async Task<Response> GetMessage(string receiverId)
        {
            var userId = _jwtToken.GetAuthenticatedUserId();

            var message = await _messageRepository.GetMessage(userId, receiverId);

            return new Response(message);
        }
    }
}
