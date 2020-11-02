namespace ServiceModels.UserModels
{
    public class UserBalanceResponseModel : BaseResponseModel
    {
        /// <summary>
        /// in cents
        /// </summary>
        public long Balance { get; set; }
    }
}
