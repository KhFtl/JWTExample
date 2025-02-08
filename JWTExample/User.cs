using System.ComponentModel.DataAnnotations;

namespace JWTExample
{
    public class User
    {
        [Required]
        public string? Login { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
