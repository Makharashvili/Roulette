using ServiceModels.RoundModels;

namespace Services.ServiceInterfaces
{
    public interface IRoundService
    {
        RoundBetResponseModel PlaceBet(int userId, RoundBetRequestModel model,string ipAddress);
    }
}
