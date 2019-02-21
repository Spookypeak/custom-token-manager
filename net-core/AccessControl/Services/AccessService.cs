using AccessControl.Models;
using System;

namespace AccessControl.Services
{
	public class AccessService: IAccessService
    {
		private readonly IJwtManager _jwtManager;
		private readonly IAccessManager _accessManager;

		public AccessService(IJwtManager jwtManager, IAccessManager accessManager)
		{
			_jwtManager = jwtManager;
			_accessManager = accessManager;
		}

		public JsonWebToken ActiveToken(string username)
		{
			var user = new { Username = username };
			if (user == null)
			{
				throw new Exception("Invalid credentials.");
			}
			var jwt = _jwtManager.Create(user.Username);
			return jwt;
		}

		public void RevokeToken() => _accessManager.Deactivate();
	}
}
