using Realtime.ChatApplication.DomianModels.Models.Dto.Messages;
using Realtime.ChatApplication.DomianModels.Models.Dto.Response;
using Realtime.ChatApplication.DomianModels.Models.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.Repository.Contracts.Messages
{
    public interface IMessageRepository
    {
        Task<Message> SendMessage(Message messages);
        Task<Message> GetMessageById(string id);
        Task UpdateMessage(Message message);
        Task DeleteMessage(Message message);
        Task<List<Message>> GetMessage(string currentUserId, string receiverId);
    }
}
