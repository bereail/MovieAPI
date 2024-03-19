// ORIGINAL
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_api.Data;
using movie_api.Model.Dto;
using movie_api.Services.Implementations;
using movie_api.Services.Interfaces;
using MOVIE_API.Models;
using MOVIE_API.Models.Enum;
using System.Security.Claims;

namespace movie_api.Controllers
{
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        
        //-----------------------------------------------------------------------------------------------------------------

        [HttpGet("getMovie/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetMovieById(int id)
        {
            var movie = _movieService.GetMovieById(id);

            if (movie == null)
            {
                return NotFound($"No se encontró ninguna película con ID {id}");
            }

            return Ok(movie);
        }




//----------------------------------------------------------------------------------------------------


 [HttpGet("grouped-by-state")]
 public IActionResult GetMoviesGroupedByState()
 {
     try
     {

         string userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

         IEnumerable<MovieDto> groupedMovies;

         if (userRole == "Admin")
         {

             var moviesGroupedByState = _movieService.GetMoviesGroupedByState();
             groupedMovies = moviesGroupedByState
                 .SelectMany(entry => entry.Value.Select(movieDto => movieDto))
                 .OrderBy(movieDto => movieDto.State).ToList();
         }
         else if (userRole == "Client")
         {

             var moviesGroupedByState = _movieService.GetMoviesGroupedByState();
             groupedMovies = moviesGroupedByState
                 .Where(entry => entry.Key == MovieState.Available)
                 .SelectMany(entry => entry.Value.Select(movieDto => movieDto))
                  .OrderBy(movieDto => movieDto.State).ToList(); 
         }
         else
         {

             return StatusCode(403, "Acceso no autorizado");
         }

         return Ok(groupedMovies);
     }
     catch (Exception ex)
     {
         Console.WriteLine($"Error al obtener películas agrupadas por estado: {ex.Message}");
         return StatusCode(500, "Error interno del servidor");
     }
 }


        //----------------------------------------------------------------------------------------------------

        [HttpPost("createMovie")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateMovie([FromBody] MoviePostDto moviePostDto)
        {
            if (moviePostDto == null)
            {
                return BadRequest("Los datos de la película son nulos.");
            }

            int newMovieId = _movieService.CreateMovie(moviePostDto);

            return Ok("Pelicula agregada correctamente");
        }


        //------------------------------------------------------------------------------------------------------------------


        [Authorize(Roles = "Admin")]
        [HttpDelete("{movieId}")]
        public IActionResult DeleteMovie(int movieId)
        {
            IActionResult result = _movieService.DeleteMovie(movieId);
            return Ok(result);
        }



        //------------------------------------------------------------------------------------------------------------------


        [Authorize(Roles = "Admin")]
        [HttpPut("update-state/{movieId}")]
        public IActionResult UpdateMovie(int movieId, [FromBody] MovieUpdateDto updatedMovieDto)
        {
            var result = _movieService.UpdateMovie(movieId, updatedMovieDto);

            if (result is OkObjectResult okResult)
            {
                return Ok(okResult.Value);
            }
            else if (result is NotFoundObjectResult notFoundResult)
            {
                return NotFound(notFoundResult.Value); 
            }
            else if (result is BadRequestObjectResult badRequestResult)
            {
                return BadRequest(badRequestResult.Value); 
            }
            else
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }


        //------------------------------------------------------------------------------------------------------------------

        [HttpGet("SearchMoviesByTitle/{title}")]
        public ActionResult<Movie> SearchMoviesByTitle(string title)
        {
            try
            {
                var movie = _movieService.SearchMoviesByTitle(title);

                if (movie == null)
                {
                    return NotFound($"No se encontró ninguna película con título {title}");
                }

                return Ok(movie);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al buscar película por título: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


    }
}



