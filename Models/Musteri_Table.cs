using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.Models
{
    public class Musteri_Table
    {
        [Key]
        public int UserId { get; set; }  //user tablosuna referans

        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
        public string? ProfilImgPath { get; set; }
        public int FavoriUstaId { get; set; } // Usta tablosuna referans
         [ForeignKey("IlinfoIlId")]
        public Iller? Ilinfo { get; set; }  // Sehir tablosuna referans
        
        [ForeignKey("IlceinfoIlceId")]
        public Ilceler? Ilceinfo { get; set; }  // Sehir tablosuna referans
        
        [ForeignKey("MahalleinfoMahalleId")]
        public Mahalleler? Mahalleinfo { get; set; }  // Sehir tablosuna referans
    }
}