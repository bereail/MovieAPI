namespace movie_api.Model.Dto
{
    public class CreateBookingModel
    {
        public int UserId { get; set; }
        public BookingDetailPostDto BookingDetail { get; set; }
    }
}
