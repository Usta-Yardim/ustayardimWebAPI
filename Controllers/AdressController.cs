using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsAPI.Models;
using UstaYardımAPI.DTO;
using UstaYardımAPI.Models;

namespace UstaYardımAPI.Controllers
{
    //localhost:5000/api/users
    [ApiController]
    [Route("/api/[controller]")]
    public class AdressController:ControllerBase
    {
        
        private readonly DataContext _contextIller;
        private readonly DataContext _contextIlceler;
        private readonly DataContext _contextMahalleler;

         public AdressController(DataContext contextIller, DataContext contextIlceler, DataContext contextMahalleler) // constructor
        {
            _contextIller = contextIller;
            _contextIlceler = contextIlceler;
            _contextMahalleler = contextMahalleler;
        }

         //localhost:5000/api/UsersProfile => GET
        [HttpGet("Iller")]
        public async Task<IActionResult> GetIller()
        {
             var Iller = await _contextIller.Iller.Select(p => IllerToDTO(p)).ToListAsync();

            return  Ok(Iller); // Iller null ise kendi değer gönder
        }
        [HttpGet("Ilceler")]
        public async Task<IActionResult> GetIlceler()
        {
             var Ilceler = await _contextIlceler.Ilceler.Select(p => IlcelerToDTO(p)).ToListAsync();

            return  Ok(Ilceler); // Ilceler null ise kendi değer gönder
        }
        [HttpGet("Mahalleler")]
        public async Task<IActionResult> GetMahalleler()
        {
             var mahalleler = await _contextMahalleler.Mahalleler.Select(p => MahallelerToDTO(p)).ToListAsync();

            return  Ok(mahalleler); // Mahalleler null ise kendi değer gönder
        }

        [HttpGet("Iller/{id}")]
        public async Task<IActionResult> GetIl(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             var Il = await _contextIller.Iller.Where(p => p.IlId == id).Select(p => IllerToDTO(p)).FirstOrDefaultAsync();// Il null değilse FirsoD çalışır
            
            if (Il == null){
                return NotFound();
            }

            return  Ok(Il); // Il null ise kendi değer gönder
        }
        [HttpGet("Ilceler/{id}")]
        public async Task<IActionResult> GetIlce(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             var Ilceler = await _contextIlceler.Ilceler.Where(p => p.IlId == id).Select(p => IlcelerToDTO(p)).ToListAsync();// Ilce null değilse FirsoD çalışır
            
            if (Ilceler == null || Ilceler.Count==0){
                return NotFound();
            }

            return  Ok(Ilceler); // Ilce null ise kendi değer gönder
        }
        [HttpGet("Mahalleler/{id}")]
        public async Task<IActionResult> GetMahalle(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
             var mahalleler = await _contextMahalleler.Mahalleler.Where(p => p.IlceId == id).Select(p => MahallelerToDTO(p)).ToListAsync();// mahalle null değilse FirsoD çalışır
            
             if (mahalleler == null || mahalleler.Count ==0){
                return NotFound();
            }
            return  Ok(mahalleler); // mahalle null ise kendi değer gönder
        }

        //localhost:5000/api/users/1 => GET
       /* [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string? eposta)
        {
            if (eposta == null)
            {
                return NotFound();
            }
             //önce where ekledik ki bulamayınca hata vermesin. dto objesini değiştirdik
            var p = await _userManager.Users.Select(p => UsersToDTO(p)).FirstOrDefaultAsync(i => i.Email == eposta);  // _products null değilse FirsoD çalışır

            if (p == null){
                return NotFound();
            }

            return  Ok(p);
        }*/


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
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int? id)
        {

            if (id == null)
            {
                NotFound();
            }
            
            var user = await _userManager.Users.FirstOrDefaultAsync(i => i.UserId == id);
            
            if (user == null){
                return NotFound();
            }
    
            _userManager.Users.Remove(user);

            try
            {
                await _userManager.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(user); 
        }*/

         private static IllerDTO IllerToDTO(Iller p){
            
            var entity = new IllerDTO();
            
            if(p != null){
                entity.IlId = p.IlId;
                entity.IlAdi = p.IlAdi;
            }
            return entity;
        }
         private static IlcelerDTO IlcelerToDTO(Ilceler p){
            
            var entity = new IlcelerDTO();
            
            if(p != null){
                entity.IlceId = p.IlceId;
                entity.IlceAdi = p.IlceAdi;
                entity.IlId = p.IlId;
                entity.IlAdi = p.IlAdi;
            }
            return entity;
        }
         private static MahallelerDTO MahallelerToDTO(Mahalleler p){
            
            var entity = new MahallelerDTO();
            
            if(p != null){
                entity.MahalleId = p.MahalleId;
                entity.MahalleAdi = p.MahalleAdi;
                entity.IlceId = p.IlceId;
                entity.IlceAdi = p.IlceAdi;
                entity.IlId = p.IlId;
                entity.IlAdi = p.IlAdi;
            }
            return entity;
        }

    }
}