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
    public static class TokenService
    {
        private static string secretKey;

        private static void setPrivateKey(IConfiguration configuration)
        {
            secretKey = configuration.GetSection("Security:SecretKey").Value;
        }

        public static string GenerateOwnerJWTWebToken(OwnerViewModel ownerInfo, IConfiguration configuration) // Owner
        {
            setPrivateKey(configuration);

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

        public static string GenerateCustomerJWTWebToken(CustomerViewModel ownerInfo, IConfiguration configuration) // Customer
        {
            //setPrivateKey(configuration);

            //var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));

            //var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //var header = new JwtHeader(credential);

            //var payload = new JwtPayload
            //{
            //   { PayloadKeyConstants.ID, ownerInfo.Id.ToString()},
            //   { PayloadKeyConstants.ROLE, ((int)RoleConstants.Customer).ToString()}
            //};

            //var secToken = new JwtSecurityToken(header, payload);
            //var handler = new JwtSecurityTokenHandler();

            //return handler.WriteToken(secToken);

            setPrivateKey(configuration);

            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                        new Claim(PayloadKeyConstants.ID, ownerInfo.Id.ToString()),
                        new Claim(PayloadKeyConstants.ROLE, ((int)RoleConstants.Customer).ToString())
            };

            var token = new JwtSecurityToken("",
                "",
                claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateAdminJWTWebToken(AdminViewModel ownerInfo, IConfiguration configuration) // Admin
        {
            setPrivateKey(configuration);

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

        public static TokenViewModel ReadJWTTokenToModel(string token, IConfiguration configuration)
        {
            string tempToken = token;

            if (token.Contains("Bearer"))
            {
                token = tempToken.Split(' ')[1];
            }

            setPrivateKey(configuration);

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

        private static SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(secretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey(),
                ValidateLifetime = false
            };
        }

        private static bool IsTokenValid(string token)
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
