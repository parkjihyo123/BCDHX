using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BCDHX.Unity.PaymentBK
{
    public class PaymentLibrary
    {
        private SortedList<String, String> _requestData = new SortedList<String, String>();
        private SortedList<String, String> _responseData = new SortedList<String, String>();
        public void AddRequestData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }
        public string CreateRequestUrl(string baseUrl)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    data.Append(kv.Key + "=" + HttpUtility.UrlEncode(kv.Value) + "&&");
                }
            }
            string queryString = data.ToString();
            baseUrl += "?" + queryString;
            //string vnp_SecureHash = Utils.Sha256(vnp_HashSecret + rawData);
           
            return baseUrl;
        }
        public void AddResponseData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            string retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}