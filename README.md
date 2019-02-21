# Custom token manager

This repository contains generic examples for API access management with JWT  in different frameworks.
This is a simple and generic example easy to implement.
Currently only there's .NET Core example.
I will soon upload the same example on another language.

## Version examples

* [.NET Core](https://github.com/Spookypeak/custom-token-manager/tree/master/net-core)
This example uses an singleton to persist tokens. Each service reboot all tokens will be removed.


### Prerequisites

This example uses .NET Core v2.0

### Installing

Download it!

```
git clone https://github.com/Spookypeak/custom-token-manager.git
```

You can use it only as library.
Only add this configurations in your startup.cs.

```
using AccessControl.Models;
using AccessControl.Services;

...
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
```

Now add this settings on your appsettings.json

```
{
	"jwt": { //these are common setting on JWT implementations
		"secretKey": "UWnU9qDyhVuscgzbQC8_oYcZ080Vi45T15Gmfc1CuIw",
		"issuer": "custom-access-identity"
	},
	"accessSettings": {
		"allowedPaths": [ //over here you can add all routes you don't want authenticated
			"/api/login/00"
		]
	}
}
```

## Running the tests

Test it only excecuting.
Make get to this path to get your token

```
http://localhost:28630/api/auth/00
```

Use your token in your request headers

```
Authorization: 'Bearer' + yourToken
```


```
http://localhost:28630/api/auth/01
```

To revoke this token make an request to this route
with or without authorization header

```
http://localhost:28630/api/auth/02
```

## See also
[Piotr Gankiewicz](https://github.com/spetz/tokenmanager-sample)
This example is based on this other.
This example uses [redis](https://redis.io/). 
