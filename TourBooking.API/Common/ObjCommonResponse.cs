namespace TourBooking.API.Common
{
    public class ObjCommonResponse<T> : CommonResponse where T : class
    {
        public ObjCommonResponse(T data, string responseCode = null, string responseMessage = null, bool success = true)
        {
            ResponseCode = responseCode;
            ResponseMessage = responseMessage;
            Success = success;
            Data = data;
        }
        public T Data { get; set; }
}
}
