using MOVIE_API.Models.Enum;

namespace movie_api.Model.Dto
{
    public class MovieGetDto
    {
        public string Title { get; set; }
        public string Director { get; set; }
        public DateTime BookingDate { get; set; }
        public MovieState? State { get; set; }
        public int IdAdmin { get; set; }
    }
}
