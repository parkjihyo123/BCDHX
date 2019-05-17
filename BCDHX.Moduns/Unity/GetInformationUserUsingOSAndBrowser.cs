using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BCDHX.Moduns.Unity
{
    public class GetInformationUserUsingOSAndBrowser
    {
        private HttpRequestBase request;
        private HttpBrowserCapabilitiesBase browserCapabilities;
        public GetInformationUserUsingOSAndBrowser(HttpRequestBase request)
        {
            this.request = request;
            this.browserCapabilities = this.request.Browser;
        }
        public string browserName
        {
            get
            {
                return browserCapabilities.Browser;
            }
        }

        public string browserVersion
        {
            get
            {
                return browserCapabilities.Version;
            }
        }
        public string ipAddress
        {
            get
            {
                return GetIpAddress(this.request);
            }
        }
        public string osName
        {
            get
            {
                return getOperatinSystemDetails(request.UserAgent);
            }
        }

        public static string getOperatinSystemDetails(string browserDetails)
        {
            try
            {
                switch (browserDetails.Substring(browserDetails.LastIndexOf("Windows ") + 11, 3).Trim())
                {
                    case "10.0":
                        return "Windows 10";
                    case "6.3":
                        return "Windows 8.1";
                    case "6.2":
                        return "Windows 8";
                    case "6.1":
                        return "Windows 7";
                    case "6.0":
                        return "Windows Vista";
                    case "5.2":
                        return "Windows XP 64-Bit Edition";
                    case "5.1":
                        return "Windows XP";
                    case "5.0":
                        return "Windows 2000";
                    default:
                        return browserDetails.Substring(browserDetails.LastIndexOf("Windows "), 14);
                }
            }
            catch
            {
                if (browserDetails.Length > 149)
                    return browserDetails.Substring(0, 149);
                else
                    return browserDetails;
            }
        }
        public  string GetIpAddress( HttpRequestBase request)
        {
            if (request.Headers["CF-CONNECTING-IP"] != null)
                return request.Headers["CF-CONNECTING-IP"];

            var ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                var addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                    return addresses[0];
            }

            return request.UserHostAddress;
        }

    }

}
