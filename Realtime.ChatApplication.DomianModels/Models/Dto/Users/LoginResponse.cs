using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.DomianModels.Models.Dto.Users
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserProfile Profile { get; set; }
    }
}
