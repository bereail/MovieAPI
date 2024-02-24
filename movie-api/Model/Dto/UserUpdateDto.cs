using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace movie_api.Model.Dto
{
    public class UserUpdateDto
    {
        [JsonIgnore]
        public int Id { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Lastname { get; set; }

        public string Name { get; set; }

        public string Pass { get; set; }

        public string Rol { get; set; }

    }
}
