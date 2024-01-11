using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UstaYardımAPI.DTO;

namespace UstaYardımAPI.Models
{
    public class AppUser:IdentityUser<int>
    {
       
        public string FullName { get; set; } = null!;
        public string UserType { get; set; } = null!;
        public DateTime KayitTarihi { get; set; }

        public static implicit operator AppUser?(UsersDTO? v)
        {
           if (v == null)
        {
            return null;
        }

        // UsersDTO'dan AppUser'a dönüşümü gerçekleştir
        return new AppUser
        {
            Id = v.UserId,
            FullName = v.FullName,
            UserType = v.UserType,
            Email = v.Email,
            PhoneNumber = v.PhoneNumber,
            // Diğer özellikleri buraya ekleyin
        };
        }

        // identity sınıfından dolayı email userıd gibi alanları ekleyemedik override istiyordu

    }
}