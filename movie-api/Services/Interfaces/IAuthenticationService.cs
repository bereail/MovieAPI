using movie_api.Model.Dto;
using MOVIE_API.Models;

namespace movie_api.Services.Interfaces
{
    public interface IAuthenticationService
    {
        //Validar si el usuario es nulo o si esta desactivado
        User? ValidateUser(AuthenticationRequestBody authenticationRequestBody);
    }
}
