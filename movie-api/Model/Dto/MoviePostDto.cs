using MOVIE_API.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace movie_api.Model.Dto
{
    public class MoviePostDto
    {
        [Required(ErrorMessage = "El campo 'Title' es requerido.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "El campo 'Director' es requerido.")]
        public string Director { get; set; }

  
        public DateTime? Date { get; set; }

        [JsonIgnore] // No envíes State al cliente, ya que será manejado internamente
        public MovieState? State { get; set; } = MovieState.Available;
    }
}
