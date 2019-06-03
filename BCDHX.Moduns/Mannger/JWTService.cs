using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCDHX.Moduns.Models;
using BCDHX.Moduns.Unity;
using Microsoft.IdentityModel.Tokens;

namespace BCDHX.Moduns.Mannger
{
    public class JWTService : IAuthService
    {
        public string SecretKey { get; set; }
        private RandomCode coderandom = new RandomCode();
        public JWTService (string SecretKey)
        {
            this.SecretKey = SecretKey;
            
        }
        public string GenerateToken(IAuthContainerModel model)
        {
            if (model == null || model.Claims == null || model.Claims.Length == 0)
                throw new ArgumentException("Arguments to create token are not valid.");
            SecurityTokenDescriptor securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(model.Claims),
                Expires = DateTime.UtcNow.AddSeconds(Convert.ToInt32(model.ExpiresMin)),
                IssuedAt = DateTime.UtcNow,
                
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.SecurityAlgorim),
                NotBefore = DateTime.UtcNow,
                
            };
            string issuser = ConfigurationManager.AppSettings["AppKey"];
            string audience = null;
            IEnumerable<Claim> claims = model.Claims;
            DateTime notBefore = DateTime.UtcNow;
            DateTime expires = DateTime.UtcNow.AddSeconds(Convert.ToInt32(60));
            SigningCredentials signingCredentials = new SigningCredentials(GetSymmetricSecurityKey(), model.SecurityAlgorim);
            var tokennew = new JwtSecurityToken(issuser,audience,null,notBefore,expires,signingCredentials);
            tokennew.Payload["jti"] = "13333";                    
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();         
            SecurityToken securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            string token = jwtSecurityTokenHandler.WriteToken(tokennew);
            return token;
        }

        public IEnumerable<Claim> GetTokenClaims(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return tokenValid.Claims;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool IsTokenVaild(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Given token is null or empty.");

            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Convert.FromBase64String(SecretKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }

       
    }
}
