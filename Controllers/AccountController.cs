using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
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
        private readonly string _imageUploadRootPath = "UsersImages"; // Ana yükleme yolu
        private readonly UserManager<AppUser> _userManager;

         public AccountController(DataContext contextUstalar, UserManager<AppUser> userManager) // constructor
        {
            _contextUstalar = contextUstalar;
             _userManager = userManager;
        }

        //localhost:5000/api/Account => GET
        [HttpGet("Ustalar")]
        public async Task<IActionResult> GetUstalar()
        {
             var Ustalar = await _contextUstalar.Ustalar.Include(u => u.User)
                                                        .Include(u => u.Ilinfo)
                                                        .Include(u => u.Ilceinfo)
                                                        .Include(u => u.Mahalleinfo)
                                                        .Include(u => u.Kategori).Select(p => AccountToDTO(p,"#account-general")).ToListAsync();

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
                                                    .Include(u => u.Kategori)
                                                    .Where(p => p.UserId == id).Select(p => AccountToDTO(p,"#account-general")).FirstOrDefaultAsync();// usta null değilse FirsoD çalışır
            
            if (usta == null){
                return NotFound();
            }

            return  Ok(usta); // usta null ise kendi değer gönder
        }
        [HttpGet("Musteri/{id}")]
        public async Task<IActionResult> GetMusteri(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var musteri = await _contextUstalar.Musteriler.Include(p => p.User)
                                                    .Include(u => u.Ilinfo)
                                                    .Include(u => u.Ilceinfo)
                                                    .Include(u => u.Mahalleinfo)
                                                    .Where(p => p.UserId == id).Select(p => MusteriToDTO(p,"#account-general")).FirstOrDefaultAsync();// usta null değilse FirsoD çalışır
            
            if (musteri == null){
                return NotFound();
            }

            return  Ok(musteri); // usta null ise kendi değer gönder
        }


        [HttpPut("{id}")]  // Kullanıcıyı update et
        public async Task<IActionResult> UpdateAccountInfoUsta(int id, AccountDTO entity)
        {
            Console.WriteLine("Metoda giriş yapıldı");
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
                                                    .Include(u => u.Kategori)
                                                    .Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (usta == null){ // user-id ve ve eposta aynı olmalı
                return NotFound();
            }
            
            var (userDirectoryPath, userProfileImagePath, userReferenceImagePath) = CreateUserImageDirectory(entity.UserId);
            
            if(entity.ActiveTabPane == "#account-general" && usta.User != null && entity.User != null){
                try{
                    if(entity.ProfilImgPath != null!){
                        var url = "http://localhost:5120/";
                        byte[] bytes = Convert.FromBase64String(entity.ProfilImgPath);
                        MemoryStream ms = new MemoryStream(bytes);
                        IFormFile file = new FormFile(ms, 0, ms.Length, "fileName", "fileName.jpg");
                        usta.ProfilImgPath = url+UploadImage(file, userProfileImagePath)?.Replace('\\','/') ?? null!;
                    }
                    
                }catch{
                    Console.WriteLine("ProfilImgPath base64 formatında değil");
                }
                usta.User.FullName = entity.User.FullName;
                usta.User.PhoneNumber = entity.User.PhoneNumber;
            }
            if(entity.ActiveTabPane == "#account-change-password"){ // burasını düzeltilecek
                //usta.User.PasswordHash = entity.User.Password;
                bool isCurrentPasswordValid = false;
                if(usta.User != null && entity.NewPassword != null && entity.OldPassword != null){
                    isCurrentPasswordValid = await _userManager.CheckPasswordAsync(usta.User, entity.OldPassword);
                
                    if (!isCurrentPasswordValid)
                    {
                        // Mevcut şifre doğrulaması başarısız
                        // Hata mesajı gönder veya işlemi durdur
                        return BadRequest("Mevcut şifre yanlış.");
                    }
                    var newPasswordHash = _userManager.PasswordHasher.HashPassword(usta.User, entity.NewPassword);
                    usta.User.PasswordHash = newPasswordHash;
                }else{
                    return BadRequest("usta.User Null gelmiş");
                }
            }

            if(entity.ActiveTabPane == "#account-galeri"){
                if(entity.ReferansImgPath != null){
                    List<string> ReferansImgPathList = new List<string>();
                    foreach(var imagePath in entity.ReferansImgPath) {
                        ProcessImageAndAddToList(imagePath, ReferansImgPathList , userReferenceImagePath);
                    }
                    UpdateReferansImgPath(usta, ReferansImgPathList); 
                }
                else{
                    usta.ReferansImgPath = null;
                }   
            }

            if(entity.ActiveTabPane == "#account-info"){

                usta.Birthday = entity.Birthday;
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
            }
            try
            {
                await _contextUstalar.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }
            
            return Ok(AccountToDTO(usta,entity.ActiveTabPane)); // status code 201 güncelledim usta bilgilerini gönder
        }
        [HttpPut("MusteriUpdate/{id}")]  // Kullanıcıyı update et
        public async Task<IActionResult> UpdateAccountInfoMusteri(int id, MusteriDTO entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Model geçerli değilse, hata durumu ve mesajını döndür
            }

            if(id != entity.UserId){
                return BadRequest(); // id yanlış olursa status code 400 bad request
            }

            var musteri = await _contextUstalar.Musteriler.Include(p => p.User)
                                                    .Include(u => u.Ilinfo)
                                                    .Include(u => u.Ilceinfo)
                                                    .Include(u => u.Mahalleinfo)
                                                    .Where(p => p.UserId == id).FirstOrDefaultAsync();

            if (musteri == null){ // user-id ve ve eposta aynı olmalı
                return NotFound();
            }
            
            var (userDirectoryPath, userProfileImagePath, userReferenceImagePath) = CreateUserImageDirectory(entity.UserId);
            
            if(entity.ActiveTabPane == "#account-general" && musteri.User != null && entity.User != null){
                try{
                    if(entity.ProfilImgPath != null!){
                        var url = "http://localhost:5120/";
                        byte[] bytes = Convert.FromBase64String(entity.ProfilImgPath);
                        MemoryStream ms = new MemoryStream(bytes);
                        IFormFile file = new FormFile(ms, 0, ms.Length, "fileName", "fileName.jpg");
                        musteri.ProfilImgPath = url+UploadImage(file, userProfileImagePath)?.Replace('\\','/') ?? null!;
                    }
                    
                }catch{
                    Console.WriteLine("ProfilImgPath base64 formatında değil");
                }
                musteri.User.FullName = entity.User.FullName;
                musteri.User.PhoneNumber = entity.User.PhoneNumber;
            }
            if(entity.ActiveTabPane == "#account-change-password"){ // burasını düzeltilecek
                //usta.User.PasswordHash = entity.User.Password;
                bool isCurrentPasswordValid = false;
                if(musteri.User != null && entity.NewPassword != null && entity.OldPassword != null){
                    isCurrentPasswordValid = await _userManager.CheckPasswordAsync(musteri.User, entity.OldPassword);
                
                    if (!isCurrentPasswordValid)
                    {
                        // Mevcut şifre doğrulaması başarısız
                        // Hata mesajı gönder veya işlemi durdur
                        return BadRequest("Mevcut şifre yanlış.");
                    }
                    var newPasswordHash = _userManager.PasswordHasher.HashPassword(musteri.User, entity.NewPassword);
                    musteri.User.PasswordHash = newPasswordHash;
                }else{
                    return BadRequest("usta.User Null gelmiş");
                }
            }

            if(entity.ActiveTabPane == "#account-info"){

               
                if(entity.Ilinfo != null){
                    musteri.Ilinfo = new Iller();
                    musteri.Ilinfo = await _contextUstalar.Iller.Where(p => p.IlId == entity.Ilinfo.IlId).FirstOrDefaultAsync();
                } 
                if(entity.Ilceinfo != null){
                    musteri.Ilceinfo = new Ilceler();
                    musteri.Ilceinfo = await _contextUstalar.Ilceler.Where(p => p.IlceId == entity.Ilceinfo.IlceId).FirstOrDefaultAsync();
                } 

                if(entity.Mahalleinfo != null){
                    musteri.Mahalleinfo = new Mahalleler();
                    musteri.Mahalleinfo = await _contextUstalar.Mahalleler.Where(p => p.MahalleId == entity.Mahalleinfo.MahalleId).FirstOrDefaultAsync();
                }
            }
            try
            {
                await _contextUstalar.SaveChangesAsync();
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok(MusteriToDTO(musteri,entity.ActiveTabPane)); // status code 201 güncelledim usta bilgilerini gönder
        }


         private static UserDTO AppUserToDTO(AppUser? p){
            
            var entity = new UserDTO();
            
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
            }
            return entity;
        }

        public static AccountDTO AccountToDTO(Usta_Table p, string ActiveTabPane){
            
            var entity = new AccountDTO();
            
    
            List<string>? ReferansImgPathsList = null;
            if(p.ReferansImgPath != "null"){
                ReferansImgPathsList = !string.IsNullOrEmpty(p.ReferansImgPath)
                ? p.ReferansImgPath.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>();
            }
            
            
            if(p != null){
                entity.UserId = p.UserId;
                entity.User = AppUserToDTO(p.User);
                if(p.ProfilImgPath != null)
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
                if(p.Kategori != null){
                    entity.KategoriId = p.Kategori.Id;
                    entity.KategoriName = p.Kategori.KategoriName;
                }
                if (ReferansImgPathsList != null)
                entity.ReferansImgPath = ReferansImgPathsList; // burasının başına hepsine url eklemek gerekicek
                entity.ActiveTabPane = ActiveTabPane;
            }
            return entity;
        }
        private static MusteriDTO MusteriToDTO(Musteri_Table p, string ActiveTabPane){
            
            var entity = new MusteriDTO();

            List<string>? FavoriUstaId = null;
            if(p.FavoriUstaId != "null"){
                FavoriUstaId = !string.IsNullOrEmpty(p.FavoriUstaId)
                ? p.FavoriUstaId.Split(',').Select(s => s.Trim()).ToList()
                : new List<string>();
            }
                 
            if(p != null){
                entity.UserId = p.UserId;
                entity.User = AppUserToDTO(p.User);
                if(p.ProfilImgPath != null)
                entity.ProfilImgPath = p.ProfilImgPath;
                if (FavoriUstaId != null)
                entity.FavoriUstaId = FavoriUstaId;
                if (p.Ilinfo != null) {
                    entity.Ilinfo = AdressController.IllerToDTO(p.Ilinfo);
                }
                if (p.Ilceinfo != null) {
                    entity.Ilceinfo = AdressController.IlcelerToDTO(p.Ilceinfo);
                }
                if (p.Mahalleinfo != null) {
                    entity.Mahalleinfo = AdressController.MahallelerToDTO(p.Mahalleinfo);
                }
                entity.ActiveTabPane = ActiveTabPane;
            }
            return entity;
        }
        
        private (string userDirectoryPath, string userProfileImagePath, string userReferenceImagePath) CreateUserImageDirectory(int userId)
        {
            var userDirectoryPath = Path.Combine(_imageUploadRootPath, userId.ToString());
            var userProfileImagePath = Path.Combine(userDirectoryPath, "ProfilImgPath");
            var userReferenceImagePath = Path.Combine(userDirectoryPath, "ReferansImgPath");

            // Kullanıcı dizinini oluştur
            Directory.CreateDirectory(userProfileImagePath);
            Directory.CreateDirectory(userReferenceImagePath);

            return (userDirectoryPath, userProfileImagePath, userReferenceImagePath);
        }

        // Resmi sunucuya yükleyen metod
        private string? UploadImage(IFormFile imageFile , string userProfileImagePath)
        {
            if (imageFile == null || imageFile.Length <= 0)
            {
                // Hata yönetimi: Geçersiz dosya
                return null;
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName; // Resim dosya adını benzersiz hale getirme
            var filePath = Path.Combine(userProfileImagePath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                imageFile.CopyTo(stream); // Dosyayı sunucuya kopyala
            }

            return filePath; // Resmin benzersiz adını döndür
        }

        void ProcessImageAndAddToList(string imagePath, List<string> ReferansImgPathList ,string userReferenceImagePath) {
            try {
                var url = "http://localhost:5120/";
                byte[] bytes = Convert.FromBase64String(imagePath);
                MemoryStream ms = new MemoryStream(bytes);
                ms.Seek(0, SeekOrigin.Begin);
                IFormFile file = new FormFile(ms, 0, ms.Length, "fileName", "fileName.jpg");
                string? filePath = url+UploadImage(file, userReferenceImagePath)?.Replace('\\','/');
                if(filePath != null){
                    ReferansImgPathList.Add(filePath);}
            } catch {
                Console.WriteLine("ReferansImgPath base64 formatında değil");
            }
        }

        // Update ReferansImgPath in Usta_Table
        void UpdateReferansImgPath(Usta_Table usta , List<string> ReferansImgPathList) {
            string referansImgPathAsString = string.Join(",", ReferansImgPathList);
            usta.ReferansImgPath = referansImgPathAsString;
        }
    }
}
