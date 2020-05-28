using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]

        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username)) 
                return BadRequest("Username alresdy exists");

            var usertoCreate = new User
            {
                UserName = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(usertoCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForRegisterDto userForRegisterDto)
        {
            var userFromRepo = await _repo.Login(userForRegisterDto.Username, userForRegisterDto.Password);

            if(userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.UserName)
            };
           

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHander = new JwtSecurityTokenHandler();

            var token = tokenHander.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHander.WriteToken(token)
            });
        }
    }
}
