using movie_api.Model.Dto;
using MOVIE_API.Models;

namespace movie_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        User? ValidateUser(AuthenticationRequestBody authenticationRequestBody);
    }
}
