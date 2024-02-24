using MOVIE_API.Models;

namespace movie_api.Model.Dto
{
    public class BookingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? BookingId { get; set; }
        public string MovieName { get; set; }
        public DateTime ReturnDate { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; }
        public bool UserNotFound { get; set; }
        public bool NoBookings { get; set; }
        public Booking Booking { get; set; }

        public int NumberState  { get; set;}
    }
}
