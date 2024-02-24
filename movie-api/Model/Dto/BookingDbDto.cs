
namespace movie_api.Model.Dto
{
    public class BookingDbDto
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public List<BookingGetDetailDto> BookingDetails { get; set; }
    }
}
