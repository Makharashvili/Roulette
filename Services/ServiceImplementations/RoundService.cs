using Common.Enums;
using Domain.Data;
using ge.singular.roulette;
using Microsoft.Extensions.Configuration;
using ServiceModels.RoundModels;
using Services.ServiceInterfaces;
using System;
using System.Linq;

namespace Services.Implementations
{
    public class RoundService : IRoundService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbContext;
        private readonly IJackpotService _jackpotService;
        public int jackPercFromBet;
        public RoundService(ApplicationDbContext dbContext, IConfiguration configuration, IJackpotService jackpotService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _jackpotService = jackpotService;
            jackPercFromBet = _configuration.GetValue<int>("Jackpot:PercFromBet");
        }

        public RoundBetResponseModel PlaceBet(int userId, RoundBetRequestModel model, string ipAddress)
        {
            // you never know when user will be deleted so maybe some check
            var user = _dbContext.Users.FirstOrDefault(x => x.DeleteDate == null && x.Id == userId);
            if (user == null)
            {
                return new RoundBetResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.NotFound,
                    DeveloperMessage = $"User not found with {userId} id",
                };
            }
            var isBetValid = CheckBets.IsValid(model.BetString);

            if (!isBetValid.getIsValid())
            {
                return new RoundBetResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.InvalidValue,
                    DeveloperMessage = $"Requested bet string invalid \"{model.BetString}\"",
                };
            }
            var betAmount = isBetValid.getBetAmount();

            if (user.Balance < betAmount)
            {
                return new RoundBetResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.BalanceNotEnough,
                    DeveloperMessage = $"Not enough balance",
                };
            }
            user.Balance -= betAmount;
            if (_dbContext.SaveChanges() > 0)
            {
                var betAmountInMilli = betAmount * 100;
                var jackToIncrease = betAmountInMilli * jackPercFromBet / 100;
                _jackpotService.IncreaseJackpot(jackToIncrease);

                var winningResult = new Random().Next(0, 36);
                int wonAmount = CheckBets.EstimateWin(model.BetString, winningResult);
                _dbContext.Rounds.Add(new Round
                {
                    IpAddress = ipAddress,
                    UserId = userId,
                    WonAmount = wonAmount,
                    BetAmount = betAmount,
                    BetString =  model.BetString,
                    CreateDate = DateTime.UtcNow,
                    WinningResult = winningResult,
                });
                user.Balance += wonAmount;
                if (_dbContext.SaveChanges() > 0)
                {
                    return new RoundBetResponseModel
                    {
                        Success = true,
                        WonAmount = wonAmount,
                    };
                }
                else
                {
                    return new RoundBetResponseModel
                    {
                        ErrorCode = ErrorCode.ChangesNotSaved,
                        DeveloperMessage = "Something went wrong and changes could not be saved.",
                    };
                }

            }
            else
            {
                return new RoundBetResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.ChangesNotSaved,
                    DeveloperMessage = $"Couldnot subtract balance from user",
                };
            }
        }
    }
}
