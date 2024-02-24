//1
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using movie_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using movie_api.Models.DTO;
using MOVIE_API.Models;
using movie_api.Model.Dto;

namespace MOVIE_API.Controllers
{
 
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;
        }



        //----------------------------------------------------------------------------------------------------------------------------------------
        //Trae un usuario por su id -> Admin

        [HttpGet("getUserById/{id}")]

        public IActionResult GetUserById(int id)
        {
            try
            {
                //  obtener el usuario por ID
                var user = _userService.GetUserById(id);

                if (user == null)
                {
                    // Devuelve 404,si el usuario no se encuentra
                    return NotFound($"No se encontró ningún usuario con ID {id}");
                }

                // Devuelve el usuario encontrado 
                return Ok(user);
            }
            catch (Exception ex)
            {
                // Error,  500 Internal Server Error
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }

        }

        //----------------------------------------------------------------------------------------------------------------------------------------
        //Obtener las booking asociadas ingresando el id user -> Admin

        [HttpGet("{userId}/bookingIds")]

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
        //Funciòn para traer todos los usuarios -> Admin

        [HttpGet("users")]

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
        // Función para obtener todos los administradores  -> Admin

        [HttpGet("getAdmins")]

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

        //Funciòn para traer a todos los clientes  -> Admin
        [HttpGet("getClients")]

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



        //----------------------------------------------------------------------------------------------------------------------------------------
        //Editar los datos de un usuario
        [HttpPut("{id}")]

        public IActionResult UpdateUser(int id, [FromBody] UserUpdateDto updatedUser)
        {
            if (updatedUser == null || id != updatedUser.Id || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userService.UpdateUser(updatedUser);

            if (result.Result)
            {
                return Ok(result);
            }
            else
            {
                return NotFound(result);
            }
        }

    }
}


