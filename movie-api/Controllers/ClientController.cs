﻿using Microsoft.AspNetCore.Mvc;
using movie_api.Models.DTO;
using movie_api.Services.Interfaces;
using MOVIE_API.Models.DTO;
using MOVIE_API.Services.Interfaces;

namespace MOVIE_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IClientService _clientService;

        public ClientController(IUserService userService, IClientService clientService)
        {
            _userService = userService;
            _clientService = clientService; 
        }


        //-----------------------------------------------------------------------------------------------------------------------------------------6
        //crear un nuevo cliente y asociarlo a un booking id  -> Cuaqluier persona


        [HttpPost("CreateClient")]
        public IActionResult CreateClient([FromBody] ClientCreateDto clientDto)
        {
            try
            {
                // Llama al servicio para crear un cliente y obtiene el ID del user creado
                int userId = _clientService.CreateClient(clientDto);

                // Verifica si el usuario se creo exitosamente
                if (userId != -1)
                {
                    // Si el usuario se creo con éxito, devuelve Ok
                    return Ok(new { Message = "Client created successfully."});
                }
                else
                {
                    // Si ya existe un usuario con el mismo email, devuelve error
                    return BadRequest(new { Message = "User with the same email already exists." });
                }
            }
            catch (Exception ex)
            {
                // Error
                return BadRequest(new { Message = $"Error creating client: {ex.Message}" });
            }
        }


    }
}
    

