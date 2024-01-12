using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using UstaYardımAPI.DTO;
using UstaYardımAPI.Models;

namespace UstaYardimAPI.DTO
{
    public class AccountDTO
    {
        [Key]
        public int UserId { get; set; }  //user tablosuna referans

        [ForeignKey("UserId")]
        public UserDTO? User { get; set; }
        public string? ProfilImgPath { get; set; } 
        public IllerDTO? Ilinfo { get; set; }  // Sehir tablosuna referans
        public IlcelerDTO? Ilceinfo { get; set; }  // Sehir tablosuna referans
        public MahallelerDTO? Mahalleinfo { get; set; }  // Sehir tablosuna referans
        public int? Puan { get; set; }
        public string? Hakkinda { get; set; }
        public DateTime? Birthday { get; set; }
        public string? TamamlananIs { get; set; }
        public List<string>? ReferansImgPath { get; set; }
        public string ActiveTabPane { get; set; } = "#account-general";
        public string? OldPassword { get; set; } 
        public string? NewPassword { get; set; }
        
    }


}