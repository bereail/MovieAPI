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


        public int CreateClient(ClientCreateDto clientDto)
        {
                try
                {
                    var existingUser = _movieDbContext.Users.SingleOrDefault(u => u.Email == clientDto.Email);

                    if (existingUser == null)
                    {
                        User newUser = new User
                        {
                            Name = clientDto.Name,
                            Lastname = clientDto.Lastname,
                            Email = clientDto.Email,
                            Pass = clientDto.Pass,
                            Rol = "Client",
                            IsActive = clientDto.IsActive,
                        };

                    Client newClient = new Client();
                    {
                        
                    };
                    

                    newUser.Clients.Add(newClient);


                    _movieDbContext.Users.Add(newUser);
                        _movieDbContext.SaveChanges();

                    _bookingService.AddNewBooking(newUser.Id);

                    return newUser.Id;  

                    }
                    else
                    {
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
