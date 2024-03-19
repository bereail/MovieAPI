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

        //-------------------------------------------------------------------------------------------------------------------------
        public bool CompareUserIdWithLoggedInUser(int id, ClaimsPrincipal user)
        {

            var userIdClaim = user.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");


            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int loggedInUserId))
            {
                var loggedInUserRoles = user.FindAll("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(r => r.Value);

                bool isAdmin = loggedInUserRoles.Contains("Admin");

                Console.WriteLine($"User ID in Claims: {loggedInUserId}, ID to compare: {id}");


                return isAdmin || (id == loggedInUserId);
            }

            return false;
        }
    }
}
