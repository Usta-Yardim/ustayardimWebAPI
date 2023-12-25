using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductsAPI.Models;
using UstaYardimAPI.DTO;
using UstaYardımAPI.Models;

namespace UstaYardimAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext  _contextUstalar;

         public AccountController(DataContext contextUstalar) // constructor
        {
            _contextUstalar = contextUstalar;
        }

        //localhost:5000/api/Account => GET
        [HttpGet("Ustalar")]
        public async Task<IActionResult> GetUstalar()
        {
             var Ustalar = await _contextUstalar.Ustalar.Include(u => u.User).ToListAsync();

            return  Ok(Ustalar); // Ustalar null ise kendi değer gönder
        }

        [HttpGet("Ustalar/{id}")]
        public async Task<IActionResult> GetUsta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             var usta = await _contextUstalar.Ustalar.Include(p => p.User).Where(p => p.UstaId == id).Select(p => AccountToDTO(p, p.User)).FirstOrDefaultAsync();// usta null değilse FirsoD çalışır
            
            if (usta == null){
                return NotFound();
            }

            return  Ok(usta); // usta null ise kendi değer gönder
        }



        /*[HttpPut("{id}")]  // Kullanıcıyı update et
        public async Task<IActionResult> UpdateUser(int id, AppUser entity)
        {

            if(id != entity.UserId){
                return BadRequest(); // id yanlış olursa status code 400 bad request
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(i => i.UserId == id);

            if (user == null){ // user-id ve ve eposta aynı olmalı
                return NotFound();
            }

            user.UserName = entity.UserName;
            user.UserSurname = entity.UserSurname;
            user.Eposta = entity.Eposta;
            user.Sifre = entity.Sifre;
            user.IlId = entity.IlId;

            try
            {
                await _userManager.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(user); // status code 204 güncelledim döndürecek bir şey yok
        }*/



        private static AccountDTO AccountToDTO(Usta_Table p, AppUser UstaUser){
            
            var entity = new AccountDTO();
            
            if(p != null){
                entity.UstaId = p.UstaId;
                entity.UserId = p.UserId;
                entity.User = UstaUser;
                entity.ProfilImgPath = p.ProfilImgPath;
                entity.Ilinfo = p.Ilinfo;
                entity.Ilceinfo = p.Ilceinfo;
                entity.Mahalleinfo = p.Mahalleinfo;
                entity.Puan = p.Puan;
                entity.Hakkinda = p.Hakkinda;
                entity.Birthday = p.Birthday;
                entity.TamamlananIs = p.TamamlananIs;
                entity.ReferansImgPath = p.ReferansImgPath;
            }
            return entity;
        }
        
    }
}