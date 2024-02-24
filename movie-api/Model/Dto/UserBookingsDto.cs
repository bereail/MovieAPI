namespace movie_api.Model.Dto
{
    public class UserBookingsDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public List<BookingGetDto> Bookings { get; set; }
    }
}
