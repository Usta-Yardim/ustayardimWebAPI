using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.Models
{
    public class Mahalleler
    {
        [Key]
        public int MahalleId { get; set; }
        public string? MahalleAdi { get; set; }
        public int IlceId { get; set; }
        public string? IlceAdi { get; set; }
        public int IlId { get; set; }
        public string? IlAdi { get; set; }
    }
}