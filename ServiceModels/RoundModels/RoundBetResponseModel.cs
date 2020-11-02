namespace ServiceModels.RoundModels
{
    public class RoundBetResponseModel : BaseResponseModel
    {
        /// <summary>
        /// in cents
        /// </summary>
        public long WonAmount { get; set; }
    }
}
