using Microsoft.AspNetCore.Mvc;
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


        [HttpPost("CreateClient")]
        public IActionResult CreateClient([FromBody] ClientCreateDto clientDto)
        {
            try
            {

                int userId = _clientService.CreateClient(clientDto);


                if (userId != -1)
                {

                    return Ok(new { Message = "Client created successfully."});
                }
                else
                {

                    return BadRequest(new { Message = "User with the same email already exists." });
                }
            }
            catch (Exception ex)
            {

                return BadRequest(new { Message = $"Error creating client: {ex.Message}" });
            }
        }


    }
}
    

