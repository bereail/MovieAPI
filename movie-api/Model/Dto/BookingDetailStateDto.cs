using MOVIE_API.Models.Enum;

namespace movie_api.Model.Dto
{
    public class BookingDetailStateDto
    {

        public string MovieTitle { get; set; }
        public BookingDetailState State { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Comment { get; set; }

        public string NewStateName { get; set; }
    }
}
