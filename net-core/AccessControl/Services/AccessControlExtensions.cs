using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AccessControl.Services
{
	public static class AccessControlExtensions
	{
		public static IApplicationBuilder UseCredentials(
			this IApplicationBuilder app
			)
		{
			app.UseMiddleware<ErrorHandlerMiddleware>();
			app.UseMiddleware<AccessManagerMiddleware>();
			return app;
		}

		public static IServiceCollection AddCrententialsAccess(
		this IServiceCollection services, bool implementsMobile = false
		)
		{
			if (implementsMobile)
				services.AddSingleton<IAccessManager, Mobile.AccessManager>();
			else
				services.AddSingleton<IAccessManager, AccessManager>();
			return AddServices(services);
		}

		private static IServiceCollection AddServices(IServiceCollection services)
		{
			services.AddSingleton<IAccessService, AccessService>();
			services.AddTransient<AccessManagerMiddleware>();
			services.AddSingleton<IJwtManager, JwtManager>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			return services;
		}

	}
}
