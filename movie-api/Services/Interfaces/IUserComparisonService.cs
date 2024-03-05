using System.Security.Claims;

namespace movie_api.Services.Interfaces
{
    public interface IUserComparisonService
    {



        //Funciòn para comprobar si el usuario autenticado coincide con el id ingresado o si es admin
        public bool CompareUserIdWithLoggedInUser(int id, ClaimsPrincipal user);
    }
}
