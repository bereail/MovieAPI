using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MOVIE_API.Models;
using movie_api.Model.Enum;
using MOVIE_API.Models.Enum;

namespace movie_api.Model.Dto
{
    public class BookingDetailPostDto
    {
        public string? MovieTitle { get; set; }
        public string? Comment { get; set; }
    }
}


