using Common.Enums;
using Domain.Data;
using Microsoft.VisualBasic;
using ServiceModels;
using ServiceModels.JackpotModels;
using Services.ServiceInterfaces;
using System;
using System.Linq;

namespace Services.Implementations
{
    public class JackpotService : IJackpotService
    {
        private readonly ApplicationDbContext _dbContext;
        public JackpotService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public JackpotModel GetJackpot()
        {
            // last jackpot entity which is not deleted seems legit to use
            var jackpot = _dbContext.Jackpots.LastOrDefault(x => x.DeleteDate == null);
            if (jackpot == null)
            {
                return new JackpotModel
                {
                    ErrorCode = ErrorCode.NotFound,
                    DeveloperMessage = "Jackpot not found"
                };
            }
            else
            {
                return new JackpotModel
                {
                    Success = true,
                    Id = jackpot.Id,
                    Amount = jackpot.Amount,
                    CreateDate = jackpot.CreateDate,
                };
            }
        }

        public BaseResponseModel IncreaseJackpot(long amountToIncrease)
        {
            var jackpot = _dbContext.Jackpots.LastOrDefault(x => x.DeleteDate == null);

            if (jackpot == null)
            {
                // if not found create new one
                _dbContext.Jackpots.Add(new Jackpot
                {
                    Amount = amountToIncrease,
                });
                if (_dbContext.SaveChanges()> 0)
                {
                    return new BaseResponseModel
                    {
                        Success = true,
                    };
                }
                return new BaseResponseModel
                {
                    Success = false,
                    ErrorCode = ErrorCode.NotFound,
                    DeveloperMessage = "Jackpot not found"
                };
            }

            jackpot.Amount += amountToIncrease;
            jackpot.UpdateDate = DateTime.UtcNow;
            if (_dbContext.SaveChanges() > 0)
            {
                return new BaseResponseModel
                {
                    Success = true,
                };
            }
            else
            {
                return new BaseResponseModel
                {
                    ErrorCode = ErrorCode.ChangesNotSaved,
                    DeveloperMessage = "Something went wrong while updating jackpot entity",
                };
            }
        }
    }
}
