using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UstaYardimAPI.DTO;
using UstaYardımAPI.DTO;
using UstaYardımAPI.Models;

namespace UstaYardımAPI.Controllers
{
    //localhost:5000/api/users
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController:ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration; //token kullanmak için secret bilgisini al
        private readonly DataContext _contextUstalar;

        public UsersController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, DataContext contextUstalar) // constructor
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _contextUstalar = contextUstalar;
        }

        //localhost:5000/api/users => GET
       /* [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
             var users = await _userManager.Users.Select(p => UsersToDTO(p)).ToListAsync();

            return  Ok(users); // users null ise kendi değer gönder
        } */

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

        [HttpPost("register")]  // Veri Ekleme db'ye
        public async Task<IActionResult> CreateUser(UsersDTO model)
        {

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser
            {
                UserName = model.Email,
                FullName = model.FullName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserType = model.UserType,
                KayitTarihi = DateTime.Now,
            };
            
            

            var result = await _userManager.CreateAsync(user, model.Password);

            if(result.Succeeded){
                var createdUser = await _contextUstalar.AppUsers.Where(p => p.Email == model.Email).Select(p => AppUserToDTO(p)).FirstOrDefaultAsync();
                

                if (model.UserType == "usta")
                {
                    var usta = new Usta_Table
                    {
                        User = createdUser,
                        UserId = createdUser.UserId,
                    };
                    _contextUstalar.Ustalar.Add(usta);
                    await _contextUstalar.SaveChangesAsync();
                }
                /*else{
                    var musteri = new Musteri_Table
                    {
                        User = createdUser,
                        UserId = createdUser.Id,
                    };
                    _contextUstalar.Musteriler.Add(musteri);
                    await _contextMusteriler.SaveChangesAsync();
                }*/

                return StatusCode(201);  // status code 201
            }

            return BadRequest(result.Errors);
            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return BadRequest(new { message = "Hatalı email girdiniz."});
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false); // false locout kapatmak - açmak için

            if(result.Succeeded)
            {
                if (user.UserType == model.UserType)
                {
                    var jwtData = GenarateJWt(user);
                    return Ok(
                        new { token = jwtData.Token,
                              ExpiresTime = jwtData.ExpiresTime.Date,
                              UserId = user.Id
                            }
                    );
                }
                else
                {
                    return Unauthorized(new { message = "Sistemde böyle bir "+model.UserType+" kaydımız bulunmamaktadır."});
                }
                
            }
            else
            {
                return Unauthorized(new { message = "Yanlış şifre girdiniz."});
            }

            //return Unauthorized(); // status code 403 yetkin yok 
        }

        private (string Token, DateTime ExpiresTime) GenarateJWt(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("AppSettings:Secret").Value ?? ""); //null ise boş string gönder
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    }
                ),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = "sefademirci.com"
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return (tokenString, tokenDescriptor.Expires ?? DateTime.MinValue);
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


        private static UsersDTO AppUserToDTO(AppUser p){
            
            var entity = new UsersDTO();
            
            if(p != null){
                entity.UserId = p.Id;
                entity.FullName = p.FullName;
                entity.UserType = p.UserType;
                entity.Email = p.Email;
                entity.PhoneNumber = p.PhoneNumber; 
            }
            return entity;
        }
    }
}