using System.ComponentModel.DataAnnotations;

namespace MOVIE_API.Models.DTO
{
    public class AdminCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Pass { get; set; }

        [Required]
        public string EmployeeNum { get; set; }
    }
}
