using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BCDHX.Moduns.Models
{
    public interface IAuthContainerModel
    {
        string SecrectKey { get; set; }
        string SecurityAlgorim { get; set; }
        int ExpiresMin { get; set; }
        Claim[] Claims { get; set; }
    }
}
