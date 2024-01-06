using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UstaYardimAPI.DTO;
using UstaYardımAPI.Controllers;
using UstaYardımAPI.DTO;
using UstaYardımAPI.Models;

namespace UstaYardimAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly DataContext  _contextUstalar;
        public string? dateTime;

         public AccountController(DataContext contextUstalar) // constructor
        {
            _contextUstalar = contextUstalar;
        }

        //localhost:5000/api/Account => GET
        [HttpGet("Ustalar")]
        public async Task<IActionResult> GetUstalar()
        {
             var Ustalar = await _contextUstalar.Ustalar.Include(u => u.User)
                                                        .Include(u => u.Ilinfo)
                                                        .Include(u => u.Ilceinfo).ToListAsync();

            return  Ok(Ustalar); // Ustalar null ise kendi değer gönder
        }

        [HttpGet("Ustalar/{id}")]
        public async Task<IActionResult> GetUsta(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var usta = await _contextUstalar.Ustalar.Include(p => p.User)
                                                    .Include(u => u.Ilinfo)
                                                    .Include(u => u.Ilceinfo)
                                                    .Include(u => u.Mahalleinfo)
                                                    .Where(p => p.UserId == id).Select(p => AccountToDTO(p, AppUserToDTO(p.User))).FirstOrDefaultAsync();// usta null değilse FirsoD çalışır
            
            if (usta == null){
                return NotFound();
            }

            if(usta.Birthday != null){
                dateTime = usta.Birthday.Value.ToString("yyyy-MM-dd");
                usta.Birthday = DateTime.Parse(dateTime);
            }

            return  Ok(usta); // usta null ise kendi değer gönder
        }



        [HttpPut("{id}")]  // Kullanıcıyı update et
        public async Task<IActionResult> UpdateAccountInfoUsta(int id, AccountDTO entity)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Model geçerli değilse, hata durumu ve mesajını döndür
            }

            if(id != entity.UserId){
                return BadRequest(); // id yanlış olursa status code 400 bad request
            }

            var usta = await _contextUstalar.Ustalar.Include(p => p.User)
                                                    .Include(u => u.Ilinfo)
                                                    .Include(u => u.Ilceinfo)
                                                    .Include(u => u.Mahalleinfo)
                                                    .Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (usta == null){ // user-id ve ve eposta aynı olmalı
                return NotFound();
            }
            if(entity.Birthday != null){
                dateTime = entity.Birthday.Value.ToString("yyyy-MM-dd");
                usta.Birthday = DateTime.Parse(dateTime);
            }
            
            usta.Hakkinda = entity.Hakkinda;
            
 
            if(entity.Ilinfo != null){
                usta.Ilinfo = new Iller();
                usta.Ilinfo = await _contextUstalar.Iller.Where(p => p.IlId == entity.Ilinfo.IlId).FirstOrDefaultAsync();
            } 
            if(entity.Ilceinfo != null){
                usta.Ilceinfo = new Ilceler();
                usta.Ilceinfo = await _contextUstalar.Ilceler.Where(p => p.IlceId == entity.Ilceinfo.IlceId).FirstOrDefaultAsync();
            } 

            if(entity.Mahalleinfo != null){
                usta.Mahalleinfo = new Mahalleler();
                usta.Mahalleinfo = await _contextUstalar.Mahalleler.Where(p => p.MahalleId == entity.Mahalleinfo.MahalleId).FirstOrDefaultAsync();
            }

            try
            {
                await _contextUstalar.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }


            return Ok(AccountToDTO(usta,AppUserToDTO(usta.User))); // status code 201 güncelledim usta bilgilerini gönder
        }


         private static UsersDTO AppUserToDTO(AppUser? p){
            
            var entity = new UsersDTO();
            
            if(p != null){
                entity.UserId = p.Id;
                entity.FullName = p.FullName;
                entity.UserType = p.UserType;
                if (p.Email != null) {
                    entity.Email = p.Email;
                }
                if (p.PhoneNumber != null) {
                    entity.PhoneNumber = p.PhoneNumber;
                }
                if (p.PasswordHash != null) {
                    entity.Password = p.PasswordHash;
                } 
            }
            return entity;
        }

        private static AccountDTO AccountToDTO(Usta_Table p, UsersDTO UstaUser){
            
            var entity = new AccountDTO();
            
            if(p != null){
                entity.UserId = p.UserId;
                entity.User = UstaUser;
                entity.ProfilImgPath = p.ProfilImgPath;
                if (p.Ilinfo != null) {
                    entity.Ilinfo = AdressController.IllerToDTO(p.Ilinfo);
                }

                if (p.Ilceinfo != null) {
                    entity.Ilceinfo = AdressController.IlcelerToDTO(p.Ilceinfo);
                }

                if (p.Mahalleinfo != null) {
                    entity.Mahalleinfo = AdressController.MahallelerToDTO(p.Mahalleinfo);
                }
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