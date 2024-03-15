using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Realtime.ChatApplication.DomianModels.Models.Dto.Response;
using Realtime.ChatApplication.DomianModels.Models.Dto.Users;
using Realtime.ChatApplication.Service.Contracts.Users;
using Realtime.ChatApplication.Service.Implementations.Auth;
using Google.Apis;
using Google.Apis.Auth;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Realtime.ChatApplication.Service.Implementations.Users
{
    public class UserService: IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtToken _jwtToken;

        public UserService(UserManager<IdentityUser> userManager, JwtToken jwtToken)
        {
            _userManager = userManager;
            _jwtToken = jwtToken;
        }

        public async Task<Response> Register(User user)
        {
            var userExists = await _userManager.FindByEmailAsync(user.Email);

            if (userExists != null) 
            {
                return new Response("Registration failed because the email is already registered.");
            }

            var newUser = new IdentityUser
            {
                UserName = user.Name,
                Email = user.Email,
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                return new Response("Registration failed" );
            }

            var Profile = new UserProfile
            {
                Id = newUser.Id,
                Name = newUser.UserName,
                Email = newUser.Email,
            };

            return new Response(Profile);
        }

        public async Task<Response> Login(Login login) 
        {
            var user = await _userManager.FindByEmailAsync(login.Email);

            if(user == null) 
            {
                return new Response("Login failed due to incorrect credentials");
            }

            var signIn = await _userManager.CheckPasswordAsync(user, login.Password);

            if(!signIn)
            {
                return new Response("Login failed due to incorrect credentials");
            }

            var Profile = new UserProfile
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email
            };

            var token = await _jwtToken.GenerateJwtToken(Profile);

            var loginResponse = new LoginResponse
            {
                Token = token,
                Profile = Profile
            };

            return new Response(loginResponse); 
        }

        public async Task<Response> GetUser()
        {
            var id = _jwtToken.GetAuthenticatedUserId();

            var user = await _userManager.Users.Where(u => u.Id != id)
                .Select(u => new UserProfile
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email
                })
                .ToListAsync();

           return new Response(user);
        }

        public async Task<Response> GetUserById(string id)
        {
            var user = await _userManager.Users.Where(u => u.Id == id)
                .Select(u => new UserProfile
                {
                    Id = u.Id,
                    Name = u.UserName,
                    Email = u.Email
                })
                .FirstOrDefaultAsync();

            return new Response(user);
        }
        public async Task<Response> SocialLogin(string token)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings();
            settings.Audience = new List<string> { "1016420993521-j4vm79o7vt2iocq60himn9hgpmp4gqbt.apps.googleusercontent.com" };

            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

                if (payload.EmailVerified)
                {
                    var user = await _userManager.FindByLoginAsync("Google", payload.Subject);

                    if (user == null)
                    {
                        user = await _userManager.FindByEmailAsync(payload.Email);

                        if (user == null)
                        {
                            user = new IdentityUser
                            {
                                UserName = payload.FamilyName,
                                Email = payload.Email,
                            };


                            var userLoginInfo = new UserLoginInfo("Goggle", payload.Subject, "Goggle");
                            var result = await _userManager.CreateAsync(user);

                            if (result.Succeeded)
                            {
                                await _userManager.AddLoginAsync(user, userLoginInfo);
                            }
                        }
                    }

                    var users = await _userManager.FindByEmailAsync(payload.Email);

                    if (users == null)
                    {
                        return null;
                    }

                    var Profile = new UserProfile
                    {
                        Id = users.Id,
                        Name = users.UserName,
                        Email = users.Email
                    };

                    var generatedToken = await _jwtToken.GenerateJwtToken(Profile);

                    var loginResponse = new LoginResponse
                    {
                        Token = generatedToken,
                        Profile = new UserProfile
                        {
                            Id = users.Id,
                            Name = users.UserName,
                            Email = users.Email
                        }
                    };

                    return new Response(loginResponse);
                }

                return null;

            }
            catch (InvalidJwtException ex)
            {
                // The token is invalid. Handle the error.
                throw new Exception("Invalid token: " + ex.Message);
            }
        }
    }
}
