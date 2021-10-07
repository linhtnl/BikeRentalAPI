using BikeRental.Business.Constants;
using BikeRental.Data.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;
        private string privateKey;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            setPrivateKey();
        }

        private void setPrivateKey()
        {
            privateKey = _configuration.GetSection("Security:PrivateKey").Value;
        }

        public string GenerateOwnerJWTWebToken(OwnerViewModel ownerInfo) // Owner
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credential);

            var payload = new JwtPayload
            {
               { PayloadKeyConstants.ID, ownerInfo.Id.ToString()},
               { PayloadKeyConstants.ROLE, ((int)RoleConstants.Owner).ToString()}
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public string GenerateCustomerJWTWebToken(CustomerViewModel ownerInfo) // Customer
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credential);

            var payload = new JwtPayload
            {
               { PayloadKeyConstants.ID, ownerInfo.Id.ToString()},
               { PayloadKeyConstants.ROLE, ((int)RoleConstants.Customer).ToString()}
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public string GenerateAdminJWTWebToken(AdminViewModel ownerInfo) // Admin
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(privateKey));

            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credential);

            var payload = new JwtPayload
            {
               { PayloadKeyConstants.ID, ownerInfo.Id.ToString()},
               { PayloadKeyConstants.ROLE, ((int)RoleConstants.Admin).ToString()}
            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(secToken);
        }

        public static TokenViewModel ReadJWTTokenToModel(string token)
        {
            var result = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Guid id = Guid.Parse(result.Claims.First(claim => claim.Type == PayloadKeyConstants.ID).Value);
            int role = int.Parse(result.Claims.First(claim => claim.Type == PayloadKeyConstants.ROLE).Value);
            return new TokenViewModel(id, role);
        }
    }
}
