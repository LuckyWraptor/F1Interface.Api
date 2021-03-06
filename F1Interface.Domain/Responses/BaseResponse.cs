using System.Text.Json.Serialization;

namespace F1Interface.Domain.Responses
{
    internal class BaseResponse<T>
    {
        public string ErrorDescription { get; set; }
        public string Message { get; set; }
        public string ResultCode { get; set; }
        [JsonPropertyName("resultObj")]
        public T Result { get; set; }
        public string Source { get; set; }
        public ulong SystemTime { get; set; }
    }
}