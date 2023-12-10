using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.DTO
{
    public class UsersDTO
    {
    
        [Required]
        public string Fullname { get; set; } = null!;
        [Key]
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string UserType { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}