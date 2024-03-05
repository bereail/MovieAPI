using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOVIE_API.Models.DTO;
using movie_api.Services.Interfaces;
using movie_api.Models.DTO;
using MOVIE_API.Models;
using MOVIE_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using movie_api.Data;
using movie_api.Model.Dto;

namespace MOVIE_API.Services.Implementations
{
    public class ClientService : IClientService
    {
        private readonly movieDbContext _movieDbContext;
        private readonly IBookingService _bookingService;  

        public ClientService(movieDbContext movieDbContext, IBookingService bookingService)
        {
            _movieDbContext = movieDbContext;
            _bookingService = bookingService;  
        }

        //Crea un nuevo cliente y lo asoocia a un nuevo booking id
        public int CreateClient(ClientCreateDto clientDto)
        {
                try
                {
                    // Verificar si el usuario ya existe y no tiene un rol de administrador
                    var existingUser = _movieDbContext.Users.SingleOrDefault(u => u.Email == clientDto.Email);

                    if (existingUser == null)
                    {
                        // Crear un nuevo objeto de tipo User y le asigna los valores del DTO
                        User newUser = new User
                        {
                            Name = clientDto.Name,
                            Lastname = clientDto.Lastname,
                            Email = clientDto.Email,
                            Pass = clientDto.Pass,
                            Rol = "Client",
                            IsActive = clientDto.IsActive,
                        };

                    // Crear un nuevo objeto de tipo Client 
                    Client newClient = new Client();
                    {
                        
                    };
                    
                    // Asocia el cliente al usuario
                    newUser.Clients.Add(newClient);

                    // Agrega el nuevo user a la db
                    _movieDbContext.Users.Add(newUser);
                        _movieDbContext.SaveChanges();

                    // Luego de crear el cliente, crea una nueva reserva asociada a ese cliente
                    _bookingService.AddNewBooking(newUser.Id);

                    return newUser.Id;  // Devolver el Id del nuevo user

                    }
                    else
                    {
                        // Usuario ya existe
                        return -1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear un administrador: {ex.Message}");
                    throw;
                }
            }


    }
    }
