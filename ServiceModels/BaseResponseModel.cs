using Common.Enums;

namespace ServiceModels
{
    public class BaseResponseModel
    {
        public bool Success { get; set; }
        public ErrorCode ErrorCode { get; set; } 
        public string DeveloperMessage { get; set; }
    }
}
