using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AccessControl.Services
{
	public class AccessManagerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IAccessManager _accessManager;

		public AccessManagerMiddleware(
			RequestDelegate next,
			IAccessManager accessManager
			)
		{
			_next = next;
			_accessManager = accessManager;
		}

		public async Task Invoke(HttpContext context)
		{
			var path = context.Request.Path.Value;

			if (_accessManager.GetAllowedPaths().Contains(path))
			{
				await _next(context);
				return;
			}
			if (_accessManager.IsActivated())
			{
				await _next(context);

				return;
			}
			else
				context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
		}
	}
}
