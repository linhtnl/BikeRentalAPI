using BikeRental.Data.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public class TokenService
    {
        public static string GenerateJWTWebToken(OwnerViewModel ownerInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("thisisprivatekey"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("id", ownerInfo.Id.ToString()),
                new Claim("fullName", ownerInfo.Fullname),
                new Claim("idenityNumber", ownerInfo.IdentityNumber),
                new Claim("mail", ownerInfo.Mail)
            };

            var token = new JwtSecurityToken("BikeRentalAPI",
                "BikeRentalAPI",
                claims,
                null, 
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
