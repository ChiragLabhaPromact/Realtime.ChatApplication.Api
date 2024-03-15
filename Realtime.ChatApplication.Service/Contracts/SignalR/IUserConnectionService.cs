using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.Service.Contracts.SignalR
{
    public interface IUserConnectionService
    {
        Task<string> GetConnectionIdAsync(string userId);
        Task AddConnectionAsync(string userId, string connectionId);
    }
}
