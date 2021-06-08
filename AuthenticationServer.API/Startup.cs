using System;
using System.Text;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.TokenGenerators;
using AuthenticationServer.API.Services.TokenValidators;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationServer.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddIdentityCore<User>(o => 
            {
                o.User.RequireUniqueEmail = true;

                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequireUppercase = false;
                o.Password.RequiredLength = 0;
            }).AddEntityFrameworkStores<AuthenticationDbContext>();

            AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
            _configuration.Bind("Authentication", authenticationConfiguration);

            //SecretClient keyVaultClient = new SecretClient(
            //    new Uri(_configuration.GetValue<string>("KeyVaultUri")),
            //    new DefaultAzureCredential());
            //authenticationConfiguration.AccessTokenSecret = keyVaultClient.GetSecret("access-token-secret").Value.Value;

            services.AddSingleton(authenticationConfiguration);

            string connectionString = _configuration.GetConnectionString("sqlite");
            services.AddDbContext<AuthenticationDbContext>(o => o.UseSqlite(connectionString));

            services.AddSingleton<AccessTokenGenerator>();
            services.AddSingleton<RefreshTokenGenerator>();
            services.AddSingleton<RefreshTokenValidator>();
            services.AddScoped<Authenticator>();
            services.AddSingleton<TokenGenerator>();
            services.AddScoped<IRefreshTokenRepository, DatabaseRefreshTokenRepository>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
                    ValidIssuer = authenticationConfiguration.Issuer,
                    ValidAudience = authenticationConfiguration.Audience,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
