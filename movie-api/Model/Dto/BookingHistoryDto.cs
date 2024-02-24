using MOVIE_API.Models.Enum;
using MOVIE_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using JsonConverter = Newtonsoft.Json.JsonConverter;

namespace movie_api.Model.Dto
{
    public class BookingHistoryDto
    {
        public DateTime BookingDate { get; set; }

        public DateTime? ReturnDate { get; set; }
        public string State { get; set; }
        public List<BookingDetailDto> BookingDetails { get; set; }
    }
}


