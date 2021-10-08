using BikeRental.Business.Constants;
using BikeRental.Data.Responses;
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
        private string secretKey;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            setPrivateKey();
        }

        private void setPrivateKey()
        {
            secretKey = _configuration.GetSection("Security:SecretKey").Value;
        }

        public string GenerateOwnerJWTWebToken(OwnerViewModel ownerInfo) // Owner
        {
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

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
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

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
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

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

        public TokenViewModel ReadJWTTokenToModel(string token)
        {
            bool isValid = IsTokenValid(token);

            if (!isValid)
            {
                throw new ErrorResponse((int)ResponseStatusConstants.UNAUTHORIZED, "Request Secret Token is invalid");
            }

            var result = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Guid id = Guid.Parse(result.Claims.First(claim => claim.Type == PayloadKeyConstants.ID).Value);
            int role = int.Parse(result.Claims.First(claim => claim.Type == PayloadKeyConstants.ROLE).Value);
            return new TokenViewModel(id, role);
        }

        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(secretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ValidateLifetime = false
            };
        }

        private bool IsTokenValid(string token)
        {
            try
            {
                ClaimsPrincipal tokenValid = new JwtSecurityTokenHandler().ValidateToken(token, GetTokenValidationParameters(), out SecurityToken validatedToken);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
