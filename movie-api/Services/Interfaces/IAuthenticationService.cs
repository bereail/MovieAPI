using movie_api.Model.Dto;
using MOVIE_API.Models;

namespace movie_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        //Validar usuario
        User? ValidateUser(AuthenticationRequestBody authenticationRequestBody);
    }
}
