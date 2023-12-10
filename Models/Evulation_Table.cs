using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.Models
{
    public class Evulation_Table
    {
        [Key]
        public int DegerlendirmeId { get; set; }
        public int UstaId { get; set; } // Usta tablosuna referans
        public string? DegerlendirmeDetaylari { get; set; }   
    }
}