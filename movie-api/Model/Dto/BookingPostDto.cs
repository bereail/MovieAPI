using MOVIE_API.Models;
using MOVIE_API.Models.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace movie_api.Model.Dto
{
    public class BookingPostDto
    {


        public int IdUser { get; set; }

        public DateTime BookingDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public int State { get; set; }

    }
}
