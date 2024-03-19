using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using movie_api.Model.Dto;
using movie_api.Models.DTO;
using movie_api.Services.Implementations;
using movie_api.Services.Interfaces;
using MOVIE_API.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MOVIE_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserComparisonService _userComparisonService;

        public AuthenticationController(IConfiguration config, IAuthenticationService authenticationService, IUserComparisonService userComparisonService)
        {
            _config = config; 
            _authenticationService = authenticationService;
            _userComparisonService = userComparisonService;
        }

        [HttpPost("authenticate")] 
        public ActionResult<string> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
           
            var user = ValidateCredentials(authenticationRequestBody);


            if (user is null)
            {
                return Unauthorized("Credenciales no válidas");
            }

            if (!user.IsActive)
            {
                return Unauthorized("La cuenta está deshabilitada");
            }


            var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"]));

            var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.Id.ToString()));
            claimsForToken.Add(new Claim("given_name", user.Email));
            claimsForToken.Add(new Claim(ClaimTypes.Role, user.Rol));

            var jwtSecurityToken = new JwtSecurityToken(
              _config["Authentication:Issuer"],
              _config["Authentication:Audience"],
              claimsForToken,
              DateTime.UtcNow,
              DateTime.UtcNow.AddHours(1),
              credentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);

            return Ok(tokenToReturn);
        }

        private User? ValidateCredentials(AuthenticationRequestBody authenticationRequestBody)
        {
            return _authenticationService.ValidateUser(authenticationRequestBody);
        }



    }
}