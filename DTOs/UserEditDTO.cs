﻿using System.ComponentModel.DataAnnotations;

namespace FarmManagerAPI.DTOs
{
    public class UserEditDTO
    {
        [Required]
        [MinLength(3)]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
