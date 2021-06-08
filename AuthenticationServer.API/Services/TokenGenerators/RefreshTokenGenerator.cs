using AuthenticationServer.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.API.Services.TokenGenerators
{
    public class RefreshTokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;
        private readonly TokenGenerator _tokenGenerator;

        public RefreshTokenGenerator(AuthenticationConfiguration configuration, TokenGenerator tokenGenerator)
        {
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
        }

        public string GenerateToken()
        {
            DateTime expirationTime = DateTime.UtcNow.AddMinutes(_configuration.RefreshTokenExpirationMinutes);

            return _tokenGenerator.GenerateToken(
                _configuration.RefreshTokenSecret,
                _configuration.Issuer,
                _configuration.Audience,
                expirationTime);
        }
    }
}
