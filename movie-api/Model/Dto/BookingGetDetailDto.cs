using MOVIE_API.Models.Enum;

namespace movie_api.Model.Dto
{
    public class BookingGetDetailDto
    {
        public int BookingDetailId { get; set; }
        public int MovieId { get; set; }
        public string Comment { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingDetailState State { get; set; }
    }
}
