

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace movie_api.Models.DTO
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string Pass { get; set; }

        public string Rol { get; set; }
    }
}

