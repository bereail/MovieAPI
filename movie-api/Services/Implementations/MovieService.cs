using Microsoft.AspNetCore.Mvc;
using movie_api.Data;
using movie_api.Services.Interfaces;
using movie_api.Model.Dto;
using MOVIE_API.Models.Enum;
using MOVIE_API.Models;
using Microsoft.EntityFrameworkCore;

namespace movie_api.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly movieDbContext _moviedbContext;

        public MovieService(movieDbContext moviedbContext)
        {
            _moviedbContext = moviedbContext;
        }

        //--------------------------------------------------------------------------------------------------------------------

        // Buscar una pelicula por su Id -> Admin
        // Encuentra todas las peliculas en todos sus estados

        public Movie? GetMovieById(int movieId)
        {

            return _moviedbContext.Movies.SingleOrDefault(u => u.Id == movieId);
        }



        //-----------------------------------------------------------------------------------------------------------------------

        // Trae TODAS las peliculas y las ordena según su state 
        // Diccionario almacena pares clave-valor, tiene MovieState como clave y List<MovieDto> como valor.
        public Dictionary<MovieState, List<MovieDto>> GetMoviesGroupedByState()
        {
            try
            {
                // Traer todas las películas desde la base de datos y mapearlas a dto
                var moviesDtos = _moviedbContext.Movies
                    .Select(MapToDto)
                    .ToList();

                // Las agrupa segun su state de las coloca en orden  descendente
                var groupedMovies = moviesDtos.GroupBy(movie => movie.State)
                    .ToDictionary(
                        group => group.Key,// La clave del diccionario es el estado
                        group => group.OrderByDescending(movie => movie.State).ToList()
                    );
                // Devuelve el diccionario resultante
                return groupedMovies;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener películas agrupadas por estado: {ex.Message}");
                return new Dictionary<MovieState, List<MovieDto>>();
            }
        }


        //Mapea un objeto Movie a un objeto MovieDto
        private MovieDto MapToDto(Movie movie)
        {
            // Crear y devolver un nuevo objeto MovieDto con propiedades mapeadas desde el objeto Movie
            return new MovieDto
            {
                Title = movie.Title,
                Director = movie.Director,
                State = (MovieState)movie.State
            };
        }


        //------------------------------------------------------------------------------------------------------------
        // Función para crear una película   -> Admin 
        public int CreateMovie(MoviePostDto moviePostDto)
        {
            var newMovie = new Movie
            {
                Title = moviePostDto.Title,
                Director = moviePostDto.Director,
                Date = moviePostDto.Date.HasValue ? moviePostDto.Date.Value : default(DateTime),
                State = moviePostDto.State
            };

            _moviedbContext.Add(newMovie);
            _moviedbContext.SaveChanges();
            return newMovie.Id;
        }

        //------------------------------------------------------------------------------------------------------------------
        //Eliminar pelicula (no elimina directo de la db, modifica su estado a no disponible) -> Admin

        public IActionResult DeleteMovie(int movieId)
        {
            // Buscar movie por su ID
            Movie movieToDelete = GetMovieById(movieId);

            if (movieToDelete != null)
            {
                if (movieToDelete.State == MovieState.Available)
                {
                    // Cambiar el state a 3 (no disponible)
                    movieToDelete.State = MovieState.NotAvailable;
                    _moviedbContext.SaveChanges();
                    return new OkObjectResult($"La película '{movieToDelete.Title}' ya no se encuentra disponible");
                }
                else if (movieToDelete.State == MovieState.Reserved)
                {
                    // No se puede eliminar una pelicula que está reservada actualmente
                    return new BadRequestObjectResult($"No se puede eliminar la película '{movieToDelete.Title}' ya que está actualmente reservada");
                }
                else
                {
                    // La película no fue encontrada
                    return new NotFoundObjectResult($"La película con ID {movieId} no fue encontrada");
                }
            }
            else
            {
                // La película no fue encontrada
                return new NotFoundObjectResult($"La película con ID {movieId} no fue encontrada");
            }
        }


        //---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Método para actualizar una película -> Admin
        public IActionResult UpdateMovie(int movieId, MovieUpdateDto updatedMovieDto)
        {
            var existingMovie = GetMovieById(movieId);

            if (existingMovie != null)
            {
                // Verifica si existingMovie.Date es nulo antes de intentar acceder a él
                existingMovie.Title = updatedMovieDto.Title ?? existingMovie.Title;
                existingMovie.Director = updatedMovieDto.Director ?? existingMovie.Director;
                existingMovie.Date = updatedMovieDto.Date ?? existingMovie.Date;


                // Guarda los cambios en la db
                _moviedbContext.SaveChanges();
                return new OkObjectResult($"La película '{existingMovie.Title}' fue actulizada exitosamente");
            }
            else
            {
              
                return new NotFoundObjectResult($"La película con ID {movieId} no fue encontrada");
            }
        }


//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
     //busca un pelicula según su title -> cualquier usuario 
     //no es case sensitive

     public Movie SearchMoviesByTitle(string title)
         {
            var existingMovie = _moviedbContext.Movies.SingleOrDefault(u => u.Title == title);

            return existingMovie;
            
        }


        //--------------------------------------------------------------------------------------------------------------------
        //Ingresando uh id de movie devuleve su tittle
        public string GetMovieTitleById(int movieId)
        {
            var (movie, state) = GetMovieAndStateById(movieId);

            if (movie != null)
            {
                return movie.Title;
            }
            else
            {
                return $"La película con ID {movieId} no fue encontrada";
            }
        }


        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Ingresando en id de movie, moodifica su state a disponible
        public void SetMovieStateToAvailable(int idMovie)
        {
            // Buscar la película por su ID
            var movie = _moviedbContext.Movies.Find(idMovie);

            if (movie != null && movie.State != MovieState.Available)
            {
                // Actualizar el estado de la película a "Available"
                movie.State = MovieState.Available;

                // Guardar los cambios en la db
                _moviedbContext.SaveChanges();
            }
            else if (movie == null)
            {
                Console.WriteLine($"No se encontró la película con ID {idMovie}.");
                
            }

            else if (movie.State == MovieState.Available)
            {
                Console.WriteLine($"La película con ID {idMovie} ya está en estado 'Available'.");

            }
        }

        //----------------------------------------------------------------------------------------------------------------------------------------------
        //Trae las peliculas sus estados
        public (Movie?, MovieState) GetMovieAndStateById(int movieId)
        {
            var movie = _moviedbContext.Movies.SingleOrDefault(u => u.Id == movieId);

            if (movie != null)
            {
                var state = (MovieState)movie.State;
                return (movie, state);
            }

            return (null, MovieState.NotAvailable);
        }






    }
}



