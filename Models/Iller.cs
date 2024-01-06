using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using UstaYardımAPI.DTO;

namespace UstaYardımAPI.Models
{
    public class Iller
    {
        [Key]
        public int IlId { get; set; }
        public string? IlAdi { get; set; }

    }

    
}