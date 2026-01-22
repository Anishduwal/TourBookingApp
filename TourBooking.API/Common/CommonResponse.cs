namespace TourBooking.API.Common
{
    public class CommonResponse(string responseCode = null, string responseMessage = null, bool success = true)
    {
        public string ResponseCode { get; set; } = responseCode;
        public string ResponseMessage { get; set; } = responseMessage;
        public bool Success { get; set; } = success;
    }
}
