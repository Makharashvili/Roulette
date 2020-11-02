using ServiceModels.RoundModels;
using System.Collections.Generic;

namespace ServiceModels.UserModels
{
    public class UserHistoryResponseModel : BaseResponseModel
    {
        public List<RoundModel> Rounds { get; set; }
    }
}
