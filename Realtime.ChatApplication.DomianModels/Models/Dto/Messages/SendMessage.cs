using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.DomianModels.Models.Dto.Messages
{
    public class SendMessage
    {
        public string receiverId {  get; set; }
        public string content { get; set; }
    }
}
