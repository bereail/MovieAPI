using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace movie_api.Model.Dto
{
    public class MovieUpdateDto
    {     
        public string Title { get; set; }

        public string Director { get; set; }

        public DateTime? Date { get; set; }



    }
}
