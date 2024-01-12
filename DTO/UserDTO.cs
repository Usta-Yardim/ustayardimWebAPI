using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.DTO
{
    public class UserDTO
    {
        public string FullName { get; set; } = null!;
        public int UserId { get; set; }
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string UserType { get; set; } = null!;
    }
}