using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCDHX.Moduns.Unity
{
    public interface IEmail
    {
         string createEmailBody(string userName, string title, string message);
       void SendHtmlFormattedEmail(string subject, string body);
    }
}
