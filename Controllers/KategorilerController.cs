using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UstaYardımAPI.Models;

namespace UstaYardimAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class KategorilerController : ControllerBase
    {
         private readonly DataContext  _context;

          public KategorilerController(DataContext context) // constructor
        {
            _context = context;
             
        }

        //localhost:5000/api/Account => GET
        [HttpGet("")]
        public async Task<IActionResult> GetKategoriler()
        {
             var kategoriler = await _context.Kategoriler.ToListAsync();

            return  Ok(kategoriler); // Ustalar null ise kendi değer gönder
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> GetKategoriUsta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var usta = await _context.Ustalar.Include(p => p.User)
                                                    .Include(u => u.Ilinfo)
                                                    .Include(u => u.Ilceinfo)
                                                    .Include(u => u.Mahalleinfo)
                                                    .Include(u => u.Kategori)
                                                    .Where(p => p.Kategori!.Id == id).Select(p => AccountController.AccountToDTO(p,"#account-general")).ToListAsync();// usta null değilse FirsoD çalışır
            
            if (usta == null){
                return NotFound();
            }

            return  Ok(usta); // usta null ise kendi değer gönder
        }
    }
}