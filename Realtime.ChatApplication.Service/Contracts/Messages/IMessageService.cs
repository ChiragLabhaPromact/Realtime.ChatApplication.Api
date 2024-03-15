using Realtime.ChatApplication.DomianModels.Models.Dto.Messages;
using Realtime.ChatApplication.DomianModels.Models.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.Service.Contracts.Messages
{
    public interface IMessageService
    {
        Task<Response> SendMessage(SendMessage send);
        Task<Response> EditMessage(string id, string content);
        Task<Response> DeleteMessage(string id);
        Task<Response> GetMessage(string receiverId);


    }
}
