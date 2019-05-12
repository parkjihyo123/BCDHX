using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCDHX.Moduns.Unity
{
    public interface IEmail
    {
       string CreateEmailBodyConfirmation(string userName, string passWord, string message,string url_action);
       void SendHtmlFormattedEmail(string subject, string body,string sendto);
    }
}
