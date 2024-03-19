//1
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movie_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using movie_api.Models.DTO;
using MOVIE_API.Models;
using movie_api.Model.Dto;
using movie_api.Services.Implementations;

namespace MOVIE_API.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
  
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBookingService _bookingService;


        public UserController(IUserService userService, IBookingService bookingService)
        {
            _userService = userService;
            _bookingService = bookingService;
        }



        //----------------------------------------------------------------------------------------------------------------------------------------

        [Authorize(Roles = "Admin")]
        [HttpGet("getUserById/{id}")]

        public IActionResult GetUserById(int id)
        {
            try
            {
               
                var user = _userService.GetUserById(id);

                if (user == null)
                {
                  
                    return NotFound($"No se encontró ningún usuario con ID {id}");
                }

               
                return Ok(user);
            }
            catch (Exception ex)
            {
              
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        //----------------------------------------------------------------------------------------------------------------------------------------


        [HttpGet("{userId}/bookingIds")]
        [Authorize(Roles = "Admin")]

        public ActionResult<List<int>> GetBookingIds(int userId)
        {
            try
            {
                var bookingIds = _userService.GetBookingIdsByUserId(userId);

                return Ok(bookingIds);
            }
            catch (Exception ex)
            {
                // Error
                return BadRequest($"Error al obtener los bookingIds: {ex.Message}");
            }
        }


        //------------------------------------------------------------------------------------------------------------------------------------------


        [HttpGet("users")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetUsers()
        {
            try
            {
                var users = _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error al obtener personas: {ex.Message}");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------


        [HttpGet("getAdmins")]
        [Authorize(Roles = "Admin")]

        public IActionResult GetAdmins()
        {
            try
            {
                var admins = _userService.GetAdmins();
                return Ok(admins);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener administradores: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------

        [HttpGet("getClients")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetClients()
        {
            try
            {
                var clients = _userService.GetClients();
                return Ok(clients);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener clientes: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }

        }




        //--------------------------------------------------------------------------------------------------------------------------------------


        [HttpPatch("{id}/reactivate")]
        public IActionResult ReactivateUser(int id)
        {
            var result = _userService.ReactivateUser(id, User);

            if (result.Result)
            {
                return Ok(new { message = result.Message });
            }
            else
            {
                return BadRequest(new { message = result.Message });
            }
        }


    }
}



