using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProductsAPI.Models
{
    public class AppUser:IdentityUser<int>
    {
       
        public string FullName { get; set; } = null!;
        public string UserType { get; set; } = null!;
        public DateTime KayitTarihi { get; set; }
    }
}