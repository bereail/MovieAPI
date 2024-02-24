using MOVIE_API.Models.Enum;

namespace movie_api.Model.Dto
{
    public class MovieDto
    {
        public string Title { get; set; }

        public string Director { get; set; }

        public MovieState State { get; set; }
    }
}
