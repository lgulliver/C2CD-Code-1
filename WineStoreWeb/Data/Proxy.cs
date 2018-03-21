using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WineStoreWeb.Data
{
    public class Proxy
    {
        protected string _endpoint;
        protected string _password;

        protected Proxy(string endpoint, string password)
        {
            if (endpoint == null || password == null)
            {
                throw new ArgumentNullException();
            }

            if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException();
            }

            _endpoint = endpoint;
            _password = password;

            if (!_endpoint.EndsWith(@"/", StringComparison.InvariantCulture)) {
                _endpoint = _endpoint + @"/";
            }

        }
    }
}
