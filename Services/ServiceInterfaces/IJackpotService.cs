using ServiceModels;
using ServiceModels.JackpotModels;

namespace Services.ServiceInterfaces
{
    public interface IJackpotService
    {
        JackpotModel GetJackpot();
        BaseResponseModel IncreaseJackpot(long amountToIncrease);
    }
}
