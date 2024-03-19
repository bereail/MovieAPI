

using movie_api.Model.Dto;
using movie_api.Services.Interfaces;
using MOVIE_API.Models;

namespace movie_api.Services.Implementations
{
    public class AuthenticateService : IAuthenticationService
    {
        private readonly IUserService _userService;

        public AuthenticateService(IUserService userService)
        {
            _userService = userService;
        }


        //---------------------------------------------------------------------------------------------------------------------------------------
        public User? ValidateUser(AuthenticationRequestBody authenticationRequestBody)
        {
            if (string.IsNullOrEmpty(authenticationRequestBody.Email) || string.IsNullOrEmpty(authenticationRequestBody.Password))
                return null;

            var user = _userService.ValidateUser(authenticationRequestBody);

            if (user is null || !user.IsActive)
            {
                return null;
            }

            return user;
        }



    }
}