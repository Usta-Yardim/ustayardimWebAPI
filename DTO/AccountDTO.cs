using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ProductsAPI.Models;
using UstaYardımAPI.DTO;
using UstaYardımAPI.Models;

namespace UstaYardimAPI.DTO
{
    public class AccountDTO
    {
        [Key]
        public int UstaId { get; set; }
        public int UserId { get; set; }  //user tablosuna referans
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
        public string? ProfilImgPath { get; set; }
        public Iller? Ilinfo { get; set; }  // Sehir tablosuna referans
        public Ilceler? Ilceinfo { get; set; }  // Sehir tablosuna referans
        public Mahalleler? Mahalleinfo { get; set; }  // Sehir tablosuna referans
        public int? Puan { get; set; }
        public string? Hakkinda { get; set; }
        public DateTime? Birthday { get; set; }
        public string? TamamlananIs { get; set; }
        public string? ReferansImgPath { get; set; }
    }

    
}