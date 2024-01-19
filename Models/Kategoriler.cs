using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UstaYardımAPI.Models
{
    public class Kategoriler
    {
        [Key]
        public int Id { get; set; }
        public string? KategoriName { get; set; }
        public string? KategoriUrl { get; set; }
    }
}