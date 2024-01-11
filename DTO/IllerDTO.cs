using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace UstaYardımAPI.DTO
{
    public class IllerDTO
    {
        [Key]
        public int IlId { get; set; }
        public string? IlAdi { get; set; }
    }
}