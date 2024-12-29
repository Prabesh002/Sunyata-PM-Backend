using Sunyata_PM_Backend.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sunyata_PM_Backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } // I should probably hash this but it's a test app so doesn't matter.

        [Required]
        public Role Role { get; set; }

    }
}
