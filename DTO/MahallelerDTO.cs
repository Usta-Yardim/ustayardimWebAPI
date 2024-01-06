using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.DTO
{
    public class MahallelerDTO
    {
        [Key]
        public int MahalleId { get; set; }
        public string? MahalleAdi { get; set; }
        
    }
}