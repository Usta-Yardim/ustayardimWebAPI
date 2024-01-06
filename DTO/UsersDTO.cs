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
        public string FullName { get; set; } = null!;
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string PhoneNumber { get; set; } = null!;
        public string UserType { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}