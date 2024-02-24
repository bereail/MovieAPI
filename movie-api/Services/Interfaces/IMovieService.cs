using Microsoft.AspNetCore.Mvc;
using movie_api.Model.Dto;
using MOVIE_API.Models;
using MOVIE_API.Models.Enum;

namespace movie_api.Services.Interfaces
{
    public interface IMovieService
    {

        //Buscar una pelicula por su Id -> Admin
        Movie? GetMovieById(int movieId);


        // trae todas las peliculs y las ordena según su state
        Dictionary<MovieState, List<MovieDto>> GetMoviesGroupedByState();


        //funcion para crear una pelicula -> Admin
        public int CreateMovie(MoviePostDto moviePostDto);

        //Eliminar pelicula (no elimina directo de la db, modifica su estado a no disponible) -> Admin
        IActionResult DeleteMovie(int movieId);


        // Modifica el estado de la película -> Admin
        public IActionResult UpdateMovie(int movieId, MovieUpdateDto updatedMovieDto);

        //busca un pelicula según su title -> cualquier usuario 
        public Movie SearchMoviesByTitle(string title);



        //Ingresando en id de moovie, moodifica su state a disponible
        public void SetMovieStateToAvailable(int idMovie);

        public (Movie?, MovieState) GetMovieAndStateById(int movieId);


        //Ingresando uh id de movie devuleve su tittle
        public string GetMovieTitleById(int movieId);
    }


}

