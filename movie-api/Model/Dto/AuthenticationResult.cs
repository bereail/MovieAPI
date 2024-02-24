using MOVIE_API.Models;

namespace movie_api.Model.Dto
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public User? User { get; set; }
    }
}
