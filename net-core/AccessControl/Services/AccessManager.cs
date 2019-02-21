using AccessControl.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AccessControl.Services
{
	public class AccessManager : IAccessManager
	{
		private ConcurrentDictionary<string, string> _credentials = new ConcurrentDictionary<string, string>();
		private readonly IHttpContextAccessor _accessor;
		private readonly IOptions<AccessOptions> _options;

		public AccessManager(
			IHttpContextAccessor accessor,
			IOptions<AccessOptions> options
			)
		{
			_accessor = accessor;
			_options = options;
		}

		public void Deactivate() => _remove();

		public bool IsActivated()
		{
			var credential = _credentials
				.FirstOrDefault(c => c.Key == _getCurrentIp() && c.Value == _getCurrentToken());
			if (!_isAvoid(credential))
				return true;
			_remove();
			return false;
		}

		public bool Activate(string token)
		{
			_remove();
			return _credentials.TryAdd(_getCurrentIp(), token);
		}

		public string[] GetAllowedPaths() => _options.Value.AllowedPaths;

		#region Private Methods
		bool _isAvoid(object tokenAccess) => tokenAccess.Equals(new KeyValuePair<string, string>());

		void _remove()
		{
			var credential = _credentials.FirstOrDefault(c => c.Value == _getCurrentToken() || c.Key == _getCurrentIp());
			_tryRemove(credential.Value);
		}

		void _tryRemove(string token)
		{
			var credential = _credentials.FirstOrDefault(c => c.Value == token);
			if (!_isAvoid(credential))
			{
				string aux = null;
				_credentials.TryRemove(credential.Key, out aux);
			}
		}

		private string _getCurrentToken()
		{
			var authorizationHeader = _accessor
				.HttpContext.Request.Headers["Authorization"];

			return authorizationHeader == StringValues.Empty
				? string.Empty
				: authorizationHeader.Single().Split(" ").Last();
		}

		private string _getCurrentIp() => _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

		#endregion
	}
}

namespace AccessControl.Services.Mobile
{
	public class AccessManager : IAccessManager
	{
		private ConcurrentDictionary<string, TokenAccess> _credentials = new ConcurrentDictionary<string, TokenAccess>();
		private readonly IHttpContextAccessor _accessor;
		private readonly IOptions<AccessOptions> _options;

		public AccessManager(
			IHttpContextAccessor accessor,
			IOptions<AccessOptions> options
			)
		{
			_accessor = accessor;
			_options = options;
		}

		public void Deactivate() => _remove();

		public bool IsActivated()
		{
			object credential = null;
			if (_isMobile())
			{
				credential = _credentials.FirstOrDefault(c => c.Value.Token == _getCurrentToken());
				if (!_isAvoid(credential))
					return true;
				return false;
			}
			credential = _credentials
				.FirstOrDefault(c => c.Key == _getCurrentIp() && c.Value.Token == _getCurrentToken());
			if (!_isAvoid(credential))
				return true;
			_remove();
			return false;
		}

		public bool Activate(string token)
		{
			_remove();
			return _credentials.TryAdd(_getCurrentIp(), new TokenAccess { IsMobile = _isMobile(), Token = token });
		}

		public string[] GetAllowedPaths() => _options.Value.AllowedPaths;

		#region Private Methods
		bool _isAvoid(object tokenAccess) => tokenAccess.Equals(default(KeyValuePair<string, TokenAccess>));

		void _remove()
		{
			var credential = default(KeyValuePair<string, TokenAccess>);
			var currentToken = _getCurrentToken();
			if (_isMobile())
			{
				credential = _credentials
					.FirstOrDefault(c => c.Value.Token == currentToken && c.Value.IsMobile);
				if (!_isAvoid(credential))
					_tryRemove(credential.Value.Token);
				else
				{
					credential = _credentials.FirstOrDefault(c => c.Value.Token == currentToken);
					_tryRemove(credential.Value.Token);
				}
			}
			else
			{
				var currentIp = _getCurrentIp();
				credential = _credentials.FirstOrDefault(c => c.Value.Token == currentToken && c.Key == currentIp);
				if (!_isAvoid(credential))
					_tryRemove(credential.Value.Token);
				else
				{
					TokenAccess aux = null;
					_credentials.TryRemove(currentIp, out aux);
				}
			}
		}

		void _tryRemove(string token)
		{
			var credential = _credentials.FirstOrDefault(c => c.Value.Token == token);
			if (!_isAvoid(credential))
			{
				TokenAccess aux = null;
				_credentials.TryRemove(credential.Key, out aux);
			}
		}

		private bool _isMobile()
		{
			var userAgent = _accessor.HttpContext.Request.Headers["User-Agent"].ToString();
			DeviceDetectorNET.DeviceDetector dd = new DeviceDetectorNET.DeviceDetector(userAgent);
			dd.DiscardBotInformation();
			dd.SkipBotDetection();
			dd.Parse();
			var deviceName = dd.GetDeviceName();
			if (userAgent.StartsWith("PostmanRuntime")) return false;
			return deviceName.Equals("smartphone");
		}

		private string _getCurrentToken()
		{
			var authorizationHeader = _accessor
				.HttpContext.Request.Headers["Authorization"];

			return authorizationHeader == StringValues.Empty
				? string.Empty
				: authorizationHeader.Single().Split(" ").Last();
		}

		private string _getCurrentIp() => _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

		#endregion

		internal class TokenAccess
		{
			public bool IsMobile { get; set; }
			public string Token { get; set; }
		}
	}
}
