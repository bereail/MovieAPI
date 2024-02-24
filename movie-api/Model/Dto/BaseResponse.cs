namespace movie_api.Models.DTO
{
    public class BaseResponse
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public bool IsSuccess { get; internal set; }
    }
}
