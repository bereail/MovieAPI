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
        //Buscar una pelicula por su Id -> Admin
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
        //Trae todas las peliculas y las ordenas segun su state
        //Solo el admin puede ver todas las peliculas
        //El user solo puede ver las pelicula disponibles
       /* [HttpGet("grouped-by-state")]
        public IActionResult GetMoviesGroupedByState()
        {
            try
            {
                // Obtener el rol del usuario desde los claims
                string userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                IEnumerable<MovieDto> groupedMovies;

                if (userRole == "Admin")
                {
                    // Usuario con rol "admin", obtiene todas las películas
                    var moviesGroupedByState = _movieService.GetMoviesGroupedByState();
                    groupedMovies = moviesGroupedByState
                        .SelectMany(entry => entry.Value.Select(movieDto => movieDto))
                        .OrderBy(movieDto => movieDto.State).ToList(); // Ordenar por estado de forma descendente
                }
                else if (userRole == "Client")
                {
                    // Usuario con rol "cliente", obtiene solo las películas con estado "available"
                    var moviesGroupedByState = _movieService.GetMoviesGroupedByState();
                    groupedMovies = moviesGroupedByState
                        .Where(entry => entry.Key == MovieState.Available)
                        .SelectMany(entry => entry.Value.Select(movieDto => movieDto))
                         .OrderBy(movieDto => movieDto.State).ToList(); // Ordenar por estado de forma descendente
                }
                else
                {
                    //error
                    return StatusCode(403, "Acceso no autorizado");
                }

                return Ok(groupedMovies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener películas agrupadas por estado: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }*/


        //prueba para el get d reat
        [HttpGet("movies")]
        public IActionResult GetMoviesGroupedByState()
        {
            try
            {
                // Obtener todas las películas agrupadas por estado
                var moviesGroupedByState = _movieService.GetMoviesGroupedByState();

                // Aquí puedes seguir con la lógica según tus necesidades sin necesidad de verificar roles.

                IEnumerable<MovieDto> groupedMovies = moviesGroupedByState
                    .SelectMany(entry => entry.Value)
                    .OrderBy(movieDto => movieDto.State)
                    .ToList();

                return Ok(groupedMovies);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener películas agrupadas por estado: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }



        //----------------------------------------------------------------------------------------------------
        //Post para crear peliculas -> Admin

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
        //Eliminar pelicula (no elimina directo de la db, modifica su estado a no disponible) -> ADMIN
        //No se podra alquilar una peliculas con el state delete

        [Authorize(Roles = "Admin")]
        [HttpDelete("{movieId}")]
        public IActionResult DeleteMovie(int movieId)
        {
            IActionResult result = _movieService.DeleteMovie(movieId);
            return Ok(result);
        }



        //------------------------------------------------------------------------------------------------------------------
        //Actualizar propiedades de una pelicula -> Admin

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
        // Endpoint para buscar películas por título

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

