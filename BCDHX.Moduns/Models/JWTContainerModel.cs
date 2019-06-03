using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BCDHX.Moduns.Models
{
    public class JWTContainerModel : IAuthContainerModel
    {
        public string SecrectKey { get; set; } = ConfigurationManager.AppSettings["SecretKey"];
        public string SecurityAlgorim { get; set; } = SecurityAlgorithms.HmacSha256;
        public int ExpiresMin { get; set; } = 60;
        public Claim[] Claims { get; set; }
    }
}
