using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.Models
{
    public class Galeri_Table
    {
        [Key]
        public int GaleriId { get; set; }
        public int UstaId { get; set; } // Usta tablosuna referans
        public string? GaleriDetaylari { get; set; }
    }
}