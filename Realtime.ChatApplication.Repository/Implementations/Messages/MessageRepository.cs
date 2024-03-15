using Microsoft.EntityFrameworkCore;
using Realtime.ChatApplication.DomianModels.Context;
using Realtime.ChatApplication.DomianModels.Models.Dto.Response;
using Realtime.ChatApplication.DomianModels.Models.Messages;
using Realtime.ChatApplication.Repository.Contracts.Messages;

namespace Realtime.ChatApplication.Repository.Implementations.Messages
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context) 
        {
            _context = context;
        }
        public async Task<Message> SendMessage(Message message)
        {
            await _context.Message.AddAsync(message);
            await _context.SaveChangesAsync();

            return message;

        }

        public async Task<Message> GetMessageById(string id)
        {
            var message = await _context.Message.Where(u => u.Id == id).FirstOrDefaultAsync();

            return message;
        }
        public async Task UpdateMessage(Message message)
        {
            _context.Message.Update(message);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteMessage(Message message)
        {
            _context.Message.Remove(message);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Message>> GetMessage(string currentUserId, string receiverId)
        {
            var message = await _context.Message.Where(u => (u.SenderId == currentUserId && u.ReceiverId == receiverId) ||
                                                            (u.SenderId == receiverId && u.ReceiverId == currentUserId))
                                                            .ToListAsync();

            return message;
        }
    }
}
