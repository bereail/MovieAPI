using movie_api.Data;
using MOVIE_API.Models;
using movie_api.Services.Interfaces;
using movie_api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_api.Model.Dto;
using movie_api.Model.Enum;
using System.Security.Claims;

namespace movie_api.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly movieDbContext _movieDbContext;
        private readonly IUserComparisonService _userComparisonService;


        public UserService(movieDbContext movieDbContext, IUserComparisonService userComparisonService)
        {
            _movieDbContext = movieDbContext;
            _userComparisonService = userComparisonService;
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        //LOGIN
        public User? ValidateUser(AuthenticationRequestBody authenticationRequestBody)
        {
            return _movieDbContext.Users.FirstOrDefault(user =>
                user.Email == authenticationRequestBody.Email && user.Pass == authenticationRequestBody.Password);
        }


            public BaseResponse Login(string email, string password)
        {
            BaseResponse response = new BaseResponse();
            User? userForLogin = GetUserByEmail(email);


            if (userForLogin != null)
            {
                if (userForLogin.Pass == password)
                {
                    response.Result = true;
                    response.Message = "loging Succesfull";
                }
                else
                {
                    response.Result = false;
                    response.Message = "wrong password";
                }
            }
            else
            {
                response.Result = false;
                response.Message = "wrong email";
            }


            return response;
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        //Trae un usuario por su id
        public User? GetUserById(int id)
        {
            return _movieDbContext.Users.SingleOrDefault(u => u.Id == id);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        //Obtener las booking asociadas ingresnado el id user -> Admin
        public List<int> GetBookingIdsByUserId(int userId)
        {
            // Utiliza LINQ para seleccionar los bookingId asociados al userId
            var bookingIds = _movieDbContext.Bookings
                .Where(b => b.IdUser == userId)
                .Select(b => b.Id)
                .ToList();

            return bookingIds;
        }



        //----------------------------------------------------------------------------------------------------------------------------------------
        //Busca un usuario por su email -> funciòn para el login
        public User? GetUserByEmail(string email)
        {
            return _movieDbContext.Users.SingleOrDefault(u => u.Email == email);


        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        // Función para obtener todas los usuarios registrados  -> Admin

        public List<UserDto> GetUsers()
        {
            try
            {
                var users = _movieDbContext.Users.ToList();
                var usersDtos = users.Select(MapToDto).ToList();
                return usersDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener usuario: {ex.Message}");
                return new List<UserDto>();
            }
        }

        // Mapeo de entidad Person a DTO
        public UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email,
                Rol = user.Rol,
            };
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        // Función para obtener todos los administradores  -> Admin

        public IEnumerable<AdminDto> GetAdmins()
        {
            try
            {
                var admins = _movieDbContext.Users
                .Where(u => u.Rol == "Admin")
            .ToList();
                var adminDtos = admins.Select(MapToAdminDto).ToList();
                return adminDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener administradores: {ex.Message}");
                return Enumerable.Empty<AdminDto>();
            }
        }

        private AdminDto MapToAdminDto(User user)
        {
            return new AdminDto
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email
            };
        }

        //-----------------------------------------------------------------------------------------------------------------------------

        //funcion para traer a todos los clientes  -> Admin

        public IEnumerable<ClientDto> GetClients()
        {
            try
            {
                // Obtener todos los usuarios con el rol "Client" desde la base de datos
                var clients = _movieDbContext.Users.Where(u => u.Rol == "Client").ToList();

                // Mapear los usuarios a objetos ClientDto usando la función MapToClientDto
                var clientDtos = clients.Select(MapToClientDto).ToList();

                // Devolver la lista de ClientDto
                return clientDtos;
            }
            catch (Exception ex)
            {
                // error
                Console.WriteLine($"Error al obtener clientes: {ex.Message}");

                // En caso de error, devolver una lista vacía de ClientDto
                return Enumerable.Empty<ClientDto>();
            }
        }

        // Función para mapear un objeto User a un objeto ClientDto
        private ClientDto MapToClientDto(User user)
        {
            return new ClientDto
            {
                Name = user.Name,
                Lastname = user.Lastname,
                Email = user.Email
            };
        }
            



        //----------------------------------------------------------------------------------------------------------------------------------------
        //Descativar un user -> Admin o mismo user
        //se consume en el service Booking
        public void DisableAccount(int userId)
        {
            // Obtener el usuario de la base de datos usando GetUserById
            var user = GetUserById(userId);

            // Verificar si el usuario existe y no está deshabilitado
            if (user != null && user.IsActive)
            {
                // Deshabilitar la cuenta
                user.IsActive = false;

                // Guardar los cambios en la base de datos
                _movieDbContext.SaveChanges();
            }

        }




        //--------------------------------------------------------------------------------------------------------------------------------------
        //Reactiar un usuario desactivado -> Admin o mismo user
        public BaseResponse ReactivateUser(int idUser, ClaimsPrincipal user)
        {
            var existingUser = GetUserById(idUser);

            if (existingUser != null)
            {
                try
                {
                    // Verificar si el usuario actual tiene permiso para reactivar la cuenta del usuario específico
                    if (!_userComparisonService.CompareUserIdWithLoggedInUser(idUser, user))
                    {
                        return new BaseResponse
                        {
                            Result = false,
                            Message = "No tienes permisos para reactivar la cuenta de este usuario."
                        };
                    }

                    // Cambiar el estado IsActive a true para reactivar el usuario
                    existingUser.IsActive = true;

                    _movieDbContext.SaveChanges();

                    return new BaseResponse
                    {
                        Result = true,
                        Message = "Usuario reactivado correctamente."
                    };
                }
                catch (Exception ex)
                {
                    return new BaseResponse
                    {
                        Result = false,
                        Message = $"Error al reactivar el usuario: {ex.Message}"
                    };
                }
            }
            else
            {
                return new BaseResponse
                {
                    Result = false,
                    Message = "El usuario no existe."
                };
            }
        }


    }
}

