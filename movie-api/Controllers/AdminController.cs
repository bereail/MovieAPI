using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MOVIE_API.Services.Interfaces;
using movie_api.Models.DTO;
using MOVIE_API.Models.DTO;
using System.Security.Claims;
using movie_api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MOVIE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }



        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        
        [HttpPost("CreateAdmin")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAdmin([FromBody] AdminCreateDto adminCreateDto)
        {
            try
            {
                _adminService.CreateAdmin(adminCreateDto);
                return Ok(new { Message = "Admin created successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Error creating admin: {ex.Message}" });
            }
        }
        


    }
}

    
