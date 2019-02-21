using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using AccessControl.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AccessControl.Services
{
    public class JwtManager : IJwtManager
    {
        private readonly JwtOptions _options;
        private readonly IAccessManager _accessManager;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _securityKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtHeader _jwtHeader;

        public JwtManager(
                        IOptions<JwtOptions> options,
            IAccessManager accessManager
			)
        {
            _options = options.Value;
			_accessManager = accessManager;
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_signingCredentials);
        }

        public JsonWebToken Create(string username)
        {
            var nowUtc = DateTime.UtcNow;
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var iat = (long)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);
            var payload = new JwtPayload
            {
                {"sub", username},
                {"iss", _options.Issuer},
                {"iat", iat},
                {"unique_name", username},
            };
            var jwt = new JwtSecurityToken(_jwtHeader, payload);
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);
			_accessManager.Activate(token);
            return new JsonWebToken
            {
                AccessToken = token,
            };
        }

		public void Revoke()
		{
			_accessManager.Deactivate();
		}
    }
}
