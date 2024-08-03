using System.ComponentModel.DataAnnotations;

namespace FarmManagerAPI.DTOs
{
    public class UserDTO
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(3)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
