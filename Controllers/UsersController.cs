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

        
        [HttpPost("register")]  // Veri Ekleme db'ye
        public async Task<IActionResult> CreateUser(RegisterDTO model)
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
                var createdUser = await _userManager.FindByEmailAsync(model.Email);
                

                if (model.UserType == "usta" && createdUser != null)
                {
                    var usta = new Usta_Table
                    {
                        User = createdUser,
                        UserId = createdUser.Id,
                        
                    };
                    _contextUstalar.Ustalar.Add(usta);
                    await _contextUstalar.SaveChangesAsync();
                     return StatusCode(201);
                }
                if(model.UserType == "musteri" && createdUser != null){

                    var musteri = new Musteri_Table
                    {
                        User = createdUser,
                        UserId = createdUser.Id,
                    };
                    _contextUstalar.Musteriler.Add(musteri);
                    await _contextUstalar.SaveChangesAsync(); //aslında contextustalar datacontexti gösterir düzeltilmeli ustalar tablosu ile alakası yok
                     return StatusCode(201);
                }
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

    }
}