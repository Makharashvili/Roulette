using Api.Helpers;
using Common.Enums;
using Domain.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceModels;
using ServiceModels.RoundModels;
using ServiceModels.UserModels;
using Services.ServiceInterfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Roulette.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user/")]
    public class UserController : BaseController    
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly JwtConfiguration _jwtConfiguration;
        private readonly IRoundService _roundService;
        public UserController(IUserService userService, SignInManager<User> signInManager, JwtConfiguration jwtConfiguration, IRoundService roundService)
        {
            _userService = userService;
            _roundService = roundService;
            _signInManager = signInManager;
            _jwtConfiguration = jwtConfiguration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<UserLoginResponseModel> Index([FromBody] UserLoginRequestModel model)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(model.Username, model.Pwd,false,false);

            if (!signInResult.Succeeded)
            {
                return new UserLoginResponseModel
                {
                    ErrorCode = ErrorCode.SimpleError,
                    DeveloperMessage = "Incorrect username or password",
                };
            }
            var user = _userService.GetUser(model.Username);
            var claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) };
            var expiresAt = DateTime.Now.AddMinutes(_jwtConfiguration.LifetimeMin);

            var token = TokenHelper.CreateToken(_jwtConfiguration.Key, expiresAt, claims);
            return new UserLoginResponseModel
            {
                Token = token,
                Success = true,
            };
        }

        [HttpGet("balance")]
        public UserBalanceResponseModel Balance()
        {
            // authorized users id will always be in claims so there is no need to check if userid has value or not
            return _userService.GetUserBalance(UserId.Value);
        }

        [HttpGet("history")]
        public UserHistoryResponseModel History()
        {
            //same comment as on 63rd line
            return _userService.GetUserHistory(UserId.Value);
        }
        
        [HttpPost("bet")]
        public RoundBetResponseModel Bet([FromBody] RoundBetRequestModel model)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            return _roundService.PlaceBet(UserId.Value, model,ipAddress);
        }
    }
}
