using System.ComponentModel.DataAnnotations;

namespace movie_api.Models.DTO
{
    public class CredentialsDto
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Pass { get; set; }
    }
}
