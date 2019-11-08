using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityAPI.Infrastructure;
using System.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Newtonsoft.Json;
using IdentityAPI.Helpers;

namespace IdentityAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private IndentityDBContext db;
        private IConfiguration config;

        public object StorageAccountHelper { get; private set; }

        public IdentityController(IndentityDBContext dbContext,IConfiguration configuration)
        {
            db = dbContext;
            config = configuration;
        }


        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [HttpPost("register",Name ="RegisterUser")]
        public async Task<ActionResult<dynamic>> Register(Users user)
        {
            TryValidateModel(user);
            if (ModelState.IsValid)
            {
                user.Status = "not verified";
                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();
                await SendVerificationMailAsync(user);
                return Created("", new
                {
                    user.Id,user.Fullname,user.Username,user.Email
                });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost("token",Name ="GetToken")]
        public ActionResult<dynamic> GetToken(LoginModel model)
        {
            TryValidateModel(model);
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(s => s.Username == model.Username && s.Password == model.Password && s.Status=="Verified");
                if(user!=null)
                {
                    var token = GenerateToken(user);
                    return Ok(new { user.Fullname, user.Email, user.Username, Token = token });
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [NonAction]
        private string GenerateToken(Users user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Fullname),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "catalogapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "patmentapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "basketapi"));
            claims.Add(new Claim(JwtRegisteredClaimNames.Aud, "orderapi"));
            //if(user.Username== "RohitOmar")
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, "admin"));
            //}
            claims.Add(new Claim(ClaimTypes.Role, user.Role));


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("Jwt:secret")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: config.GetValue<string>("Jwt:issuer"),
                audience:null,
                claims:claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials:credentials
                );
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        [NonAction]
        private async Task SendVerificationMailAsync(Users user)
        {
            var userObj = new
            {
                user.Id,
                user.Fullname,
                user.Email,
                user.Username
            };
            var messageText = JsonConvert.SerializeObject(userObj);
            StorageAccountHelper storageHelper = new StorageAccountHelper();
            storageHelper.StorageConnectionString = config.GetConnectionString("StorageConnection");
            await storageHelper.SendMessageAsync(messageText, "users");

        }
    }
}