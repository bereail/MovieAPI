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
        public User? GetUserById(int id)
        {
            return _movieDbContext.Users.SingleOrDefault(u => u.Id == id);
        }


        //----------------------------------------------------------------------------------------------------------------------------------------
        public List<int> GetBookingIdsByUserId(int userId)
        {
            var bookingIds = _movieDbContext.Bookings
                .Where(b => b.IdUser == userId)
                .Select(b => b.Id)
                .ToList();

            return bookingIds;
        }



        //----------------------------------------------------------------------------------------------------------------------------------------
        public User? GetUserByEmail(string email)
        {
            return _movieDbContext.Users.SingleOrDefault(u => u.Email == email);


        }


        //----------------------------------------------------------------------------------------------------------------------------------------

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


        public IEnumerable<ClientDto> GetClients()
        {
            try
            {

                var clients = _movieDbContext.Users.Where(u => u.Rol == "Client").ToList();

                var clientDtos = clients.Select(MapToClientDto).ToList();


                return clientDtos;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener clientes: {ex.Message}");

                return Enumerable.Empty<ClientDto>();
            }
        }

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

        public void DisableAccount(int userId)
        {
            var user = GetUserById(userId);


            if (user != null && user.IsActive)
            {

                user.IsActive = false;


                _movieDbContext.SaveChanges();
            }

        }




        //--------------------------------------------------------------------------------------------------------------------------------------

        public BaseResponse ReactivateUser(int idUser, ClaimsPrincipal user)
        {
            var existingUser = GetUserById(idUser);

            if (existingUser != null)
            {
                try
                {

                    if (!_userComparisonService.CompareUserIdWithLoggedInUser(idUser, user))
                    {
                        return new BaseResponse
                        {
                            Result = false,
                            Message = "No tienes permisos para reactivar la cuenta de este usuario."
                        };
                    }

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

