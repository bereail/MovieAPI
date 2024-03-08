using movie_api.Model.Dto;
using movie_api.Models.DTO;
using MOVIE_API.Models;
using System.Security.Claims;

namespace movie_api.Services.Interfaces
{
    public interface IUserService
    {

        //LOGIN
        User? ValidateUser(AuthenticationRequestBody authenticationRequestBody);

        //LOGIN
        public BaseResponse Login(string mail, string password);


        //Trae un usuario por su id -> Admin
        User? GetUserById(int id);


        //Obtener las booking asociadas ingresnado el id user -> Admin
        List<int> GetBookingIdsByUserId(int userId);


        //Busca un usuario por su email -> funciòn para el login
        public User? GetUserByEmail(string email);


        //Funciòn para traer todas los usuarios registrados -> Admin
        List<UserDto> GetUsers();


        //Traer todos los admins -> Admin
        public IEnumerable<AdminDto> GetAdmins();

        //Traer todos los clientes -> Admin
        public IEnumerable<ClientDto> GetClients();


        //Descativar un user -> Admin o mismo user
        public void DisableAccount(int userId);

        //Reactivar un usuario  -> Admin o mismo user
        BaseResponse ReactivateUser(int idUser, ClaimsPrincipal user);

    }
}
