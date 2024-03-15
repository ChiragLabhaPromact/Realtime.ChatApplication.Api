using Realtime.ChatApplication.DomianModels.Models.Dto.Response;
using Realtime.ChatApplication.DomianModels.Models.Dto.Users;

namespace Realtime.ChatApplication.Service.Contracts.Users
{
    public interface IUserService
    {
        Task<Response> Register(User user);
        Task<Response> Login(Login login);
        Task<Response> GetUser();
        Task<Response> GetUserById(string id);
        Task<Response> SocialLogin(string token);

    }
}
