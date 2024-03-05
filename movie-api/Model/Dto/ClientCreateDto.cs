using System.Text.Json.Serialization;

namespace MOVIE_API.Models.DTO
{
    public class ClientCreateDto
    {

            public string Name { get; set; }
            public string Lastname { get; set; }
            public string Email { get; set; }
            public string Pass { get; set; }

        [JsonIgnore]
        // Establecer IsActive a true por defecto
        public bool IsActive { get; set; } = true;
    }

}

