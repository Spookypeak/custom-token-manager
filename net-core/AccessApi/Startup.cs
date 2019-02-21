using System;
using System.Text;
using AccessControl.Models;
using AccessControl.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace AccessApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
			var jwtSection = Configuration.GetSection("jwt");
			var jwtOptions = new JwtOptions();
			jwtSection.Bind(jwtOptions);
			services.AddAuthentication()
				.AddJwtBearer(cfg =>
				{
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;

                    cfg.TokenValidationParameters = new TokenValidationParameters
					{
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
						ValidIssuer = jwtOptions.Issuer
					};
				});
			services.Configure<JwtOptions>(jwtSection);
			services.Configure<AccessOptions>(Configuration.GetSection("accessSettings"));
			/**
			 * this example also manage the access with mobile devices
			 * only you need pass `true` on `AddCrententialsAccess` method
			 * */ //services.AddCrententialsAccess(true);
			services.AddCrententialsAccess();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider prov)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
			app.UseAuthentication();
			app.UseCredentials();
            app.UseMvc();
        }
    }
}
