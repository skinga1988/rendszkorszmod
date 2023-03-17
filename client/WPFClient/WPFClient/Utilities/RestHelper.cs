using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http;

namespace WPFClient.Utilities
{
    internal class RestHelper
    {
        private static string serverURI;

        static RestHelper()
        {
            string protocol = ConfigurationManager.AppSettings["protocol"];
            serverURI = protocol + "://" + ConfigurationManager.AppSettings["serverAddress"];
        }
        static public HttpClient GetRestClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(serverURI);
            return client;
        }
    }
}
