using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.API.Models.Responses
{
    public class AuthenticatedUserResponse
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpirationTime { get; set; }
        public string RefreshToken { get; set; }
    }
}
