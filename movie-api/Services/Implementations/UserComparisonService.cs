using movie_api.Data;
using movie_api.Services.Interfaces;
using System.Security.Claims;

namespace movie_api.Services.Implementations
{
    public class UserComparisonService : IUserComparisonService
    {
        private readonly movieDbContext _moviedbContext;

        public UserComparisonService(movieDbContext moviedbContext)
        {
            _moviedbContext = moviedbContext;
        }


        //Funcion para comprobar si el usuario autenticado coincide con el id ingresado o si es admin
        public bool CompareUserIdWithLoggedInUser(int id, ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int loggedInUserId))
            {
                // Obtener los roles del usuario logeado
                var loggedInUserRoles = user.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(r => r.Value);

                // Verificar si el usuario es un administrador
                bool isAdmin = loggedInUserRoles.Contains("Admin");

                // Dentro de CompareUserIdWithLoggedInUser
                Console.WriteLine($"User ID in Claims: {loggedInUserId}, ID to compare: {id}");


                // Comparar el ID ingresado con el ID del usuario logeado o verificar si es un administrador
                return isAdmin || (id == loggedInUserId);
            }

            return false;
        }
    }
}
