using BCDHX.Moduns.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BCDHX.Moduns.Mannger
{
    public interface IAuthService
    {
        string SecretKey { get; set; }
        bool IsTokenVaild(string token);
        string GenerateToken(IAuthContainerModel model);
        IEnumerable<Claim> GetTokenClaims (string token );
    }
}
