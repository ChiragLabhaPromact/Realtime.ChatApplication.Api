using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Realtime.ChatApplication.DomianModels.Models.Dto.Messages;
using Realtime.ChatApplication.Service.Contracts.Messages;

namespace Realtime.ChatApplication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController: ControllerBase
    {
        private readonly IMessageService _messageService;
        public MessageController(IMessageService messageService) 
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(SendMessage send)
        {
            var result = await _messageService.SendMessage(send);

            if(result.IsSuccess) 
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditMessage(string id, UpdateMessage message)
        {
            var result = await _messageService.EditMessage(id, message.content);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);

        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(string id)
        {
            var result = await _messageService.DeleteMessage(id);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetMessage(string id)
        {
            var result = await _messageService.GetMessage(id);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }
    }
}
