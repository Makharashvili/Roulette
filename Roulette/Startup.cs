using Api.Helpers;
using Domain.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Roulette.Helpers;
using ServiceModels;
using Services.Implementations;
using Services.ServiceInterfaces;
using Services.SignalrHubs;
using System;
using System.Text;

namespace Roulette
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddNswagDocument();

            services.AddIdentity<User, UserRole>(o =>
            {
                o.Password = new PasswordOptions
                {
                    RequireDigit = false,
                    RequireLowercase = false,
                    RequireNonAlphanumeric = false,
                    RequireUppercase = false
                };
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddSingleton(new Func<IServiceProvider, JwtConfiguration>(GetJwtOptions));
            services.AddHttpContextAccessor();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoundService, RoundService>();
            services.AddScoped<IJackpotService, JackpotService>();
            services.AddSignalR();
            //services.AddHostedService<JackpotHostedService>();

        }

        private JwtConfiguration GetJwtOptions(IServiceProvider arg)
        {
            return new JwtConfiguration
            {
                Key = Configuration.GetValue<string>("Jwt:Key"),
                LifetimeMin = Configuration.GetValue<int>("Jwt:LifetimeMin"),
            };
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext dbContext,
            IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGlobalExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseReDoc(configuration => configuration.Path = string.Empty);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseGlobalTokenMiddleware();

            app.UseGlobalTokenMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ServiceHub>("/ServiceHub");
            });

            DbContextExtensions.Seed(dbContext, serviceProvider);
        }
    }
}
