using MOVIE_API.Models.Enum;
using System.Text.Json.Serialization;

namespace movie_api.Model.Dto
{
    public class BookingDetailDto
    {
        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public int IdMovie{ get; set; }

        public string MovieTitle { get; set; }

        [JsonIgnore]
        public BookingDetailState? State { get; set; }

        [JsonIgnore]
        public DateTime? BookingDate { get; set; }

        [JsonIgnore]
        public DateTime? ReturnDate { get; set; }
        public string Comment { get; set; }
    }
}
