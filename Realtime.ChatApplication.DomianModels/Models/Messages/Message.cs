using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.DomianModels.Models.Messages
{
    public class Message
    {
        public string Id { get; set; }
        public string SenderId { get; set; }
        public IdentityUser Sender { get; set; }
        public string ReceiverId { get; set; }
        public IdentityUser Receiver { get; set; }
        public string content { get; set; }
        public DateTime Timestemp { get; set; }
    }
}
