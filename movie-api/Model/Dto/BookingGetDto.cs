namespace movie_api.Model.Dto
{
    //GetMovieById(int movieId)
    public class BookingGetDto
    {
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public List<BookingGetDetailDto> BookingDetails { get; set; }
    }
}

