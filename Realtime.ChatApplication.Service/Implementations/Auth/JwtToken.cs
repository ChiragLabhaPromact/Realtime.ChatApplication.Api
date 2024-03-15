using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Realtime.ChatApplication.DomianModels.Models.Dto.Users;
using Realtime.ChatApplication.Service.OptionConfigurationModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Realtime.ChatApplication.Service.Implementations.Auth
{
    public class JwtToken
    {
        private readonly JwtConfigurationOptions _jwtSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtToken(IOptions<JwtConfigurationOptions> jwtSettings, IHttpContextAccessor httpContextAccessor) 
        {
            _jwtSettings = jwtSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GenerateJwtToken(UserProfile applicationUser)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, applicationUser.Id),
                new Claim(ClaimTypes.Name, applicationUser.Name),
                new Claim(ClaimTypes.Email, applicationUser.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
               issuer: _jwtSettings.Issuer,
               audience: _jwtSettings.Audience,
               claims: claims,
               expires: DateTime.Now.AddDays(1),
               signingCredentials: creds
           );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GetAuthenticatedUserId()
        {
            return GetClaimValue(ClaimTypes.NameIdentifier);
        }
        private string GetClaimValue(string claimType)
        {
            var user = _httpContextAccessor.HttpContext.User;
            var claim = user.Claims.First(c => c.Type == claimType);
            return claim?.Value;
        }
    }
}
