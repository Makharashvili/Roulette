using System;

namespace ServiceModels.JackpotModels
{
    public class JackpotModel : BaseResponseModel
    {
        public int Id { get; set; }
        /// <summary>
        /// amount in millicents
        /// </summary>
        public long Amount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
