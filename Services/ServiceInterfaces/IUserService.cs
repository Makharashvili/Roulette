using ServiceModels.UserModels;

namespace Services.ServiceInterfaces
{
    public interface IUserService
    {
        UserModel GetUser(string userName);
        UserBalanceResponseModel GetUserBalance(int userId);
        UserHistoryResponseModel GetUserHistory(int userId);
    }
}
