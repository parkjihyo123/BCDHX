using BCDHX.Models.ModelObject;
using BCDHX.Moduns.Unity;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace BCDHX.JWT
{

    public class JWTService
    {

        /// <summary>
        /// Generates token by given model.
        /// Validates whether the given model is valid, then gets the symmetric key.
        /// Encrypt the token and returns it.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Generated token.</returns>
        public string GenerateToken(PaymentWithBKOnline paymentModel )
        {
            var TockenID = new RandomCode().RandomNumber(4);
            var ExpireSeconds = 60;
            var SecretKeyValue = ConfigurationManager.AppSettings["SecretKey"];
            var APIKey = ConfigurationManager.AppSettings["AppKey"];
            var payload = new Dictionary<string, object>
        {
                {"iss", APIKey},
                {"iat", DateTimeOffset.Now.ToUnixTimeSeconds()},
                {"exp", DateTimeOffset.Now.AddSeconds(ExpireSeconds).ToUnixTimeSeconds()},
                {"jti", TockenID},
                {"form_params", paymentModel}
         };
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, SecretKeyValue);
           
            return token;
        }

        /// <summary>
        /// Receives the claims of token by given token as string.
        /// </summary>
        /// <remarks>
        /// Pay attention, one the token is FAKE the method will throw an exception.
        /// </remarks>
        /// <param name="token"></param>
        /// <returns>IEnumerable of claims for the given token.</returns>
        //public IEnumerable<Claim> GetTokenClaims(string token)
        //{
        //    if (string.IsNullOrEmpty(token))
        //        throw new ArgumentException("Given token is null or empty.");

        //    TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters();

        //    JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        //    try
        //    {
        //        ClaimsPrincipal tokenValid = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
        //        return tokenValid.Claims;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}


        

        
       
    }
}