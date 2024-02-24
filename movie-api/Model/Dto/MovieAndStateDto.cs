using MOVIE_API.Models.Enum;

namespace movie_api.Model.Dto
{
    public class MovieAndStateDto
    {
        public int Id { get; set; }

        public int? IdAdmin { get; set; }

        public string Title { get; set; }

        public string Director { get; set; }
        public DateTime? Date { get; set; }

        public MovieState State { get; set; }
    }
}
