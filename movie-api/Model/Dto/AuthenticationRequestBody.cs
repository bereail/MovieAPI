using System.ComponentModel.DataAnnotations;

namespace movie_api.Model.Dto
{
    public class AuthenticationRequestBody
    {

        [Required]
        [EmailAddress]
        public string? Email{ get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
