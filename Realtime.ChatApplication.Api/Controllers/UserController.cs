using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Realtime.ChatApplication.DomianModels.Models.Dto.Users;
using Realtime.ChatApplication.Service.Contracts.Users;

namespace Realtime.ChatApplication.Api.Controllers
{
    [Route("api")]
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService) 
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(User user)
        {
            var result = await _userService.Register(user);

            if(result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(Login login) 
        {
            var result = await _userService.Login(login);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }

        [Authorize]
        [HttpGet("users")]
        public async Task<IActionResult> GetUser()
        {
            var result = await _userService.GetUser();

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data); 

        }

        [Authorize]
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _userService.GetUserById(id);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }

        [HttpPost("SocialLogin")]
        public async Task<IActionResult> SocialLogin(TokenRequest token)
        {
            var result = await _userService.SocialLogin(token.Token);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.Data);
        }
    }
}
