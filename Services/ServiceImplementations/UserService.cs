using Common.Enums;
using Domain.Data;
using Microsoft.EntityFrameworkCore;
using ServiceModels.UserModels;
using Services.ServiceInterfaces;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public UserModel GetUser(string userName)
        {
            // without checks becouse its used only once and only after signing in with this username 
            // so till i use this only for this reason its ok 
            var user = _dbContext.Users.First(x => x.UserName == userName);
            return new UserModel
            {
                Id = user.Id,
                Username = user.UserName,
            };
        }

        public UserBalanceResponseModel GetUserBalance(int userId)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id == userId && x.DeleteDate == null);
            if (user == null)
            {
                return new UserBalanceResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.NotFound,
                    DeveloperMessage = $"User not found with {userId} id",
                };
            }
            else
            {
                return new UserBalanceResponseModel
                {
                    Success = true,
                    Balance = user.Balance
                };
            }
        }

        public UserHistoryResponseModel GetUserHistory(int userId)
        {
            var user = _dbContext.Users.Include(x => x.Rounds).FirstOrDefault(x => x.DeleteDate == null && x.Id == userId);
            if (user == null)
            {
                return new UserHistoryResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.NotFound,
                    DeveloperMessage = $"User not found with {userId} id",
                };
            }
            else
            {
                return new UserHistoryResponseModel
                {
                    Success = true,
                    Rounds = user.Rounds.Select(x=> new ServiceModels.RoundModels.RoundModel
                    {
                        Id = x.Id,
                        BetAmount = x.BetAmount,
                        BetString = x.BetString,
                        IpAddress = x.IpAddress,
                        WonAmount = x.WonAmount,
                        CreateDate = x.CreateDate,
                        Winningresult = x.WinningResult,
                    }).OrderBy(x=> x.CreateDate).ToList()
                };
            }
        }
    }
}
