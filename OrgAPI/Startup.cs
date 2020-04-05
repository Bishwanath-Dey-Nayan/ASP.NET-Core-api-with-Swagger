using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OrgDAL;

namespace OrgAPI
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

            //For Cookie based Authentication
            //services.AddCors(
            //    x => x.AddPolicy("myPolicy",
            //    p => p.AllowAnyHeader()
            //    .AllowAnyMethod()
            //    .AllowAnyOrigin()
            //    .AllowCredentials()
            //    )
            //    );

            //For handling exception Application Level
            //services.AddMvc(config => config.Filters.Add(new AuthorizeFilter()));

            services.AddMvc();




            services.AddIdentity<IdentityUser, IdentityRole>().
            AddEntityFrameworkStores<OrganizationDbContext>().
            AddDefaultTokenProviders();

            //Giving status code for unauthorized Acccss
            //services.ConfigureApplicationCookie(opt => {

            //    opt.Events = new CookieAuthenticationEvents {
            //    OnRedirectToLogin = redirectContext =>
            //    {
            //        redirectContext.HttpContext.Response.StatusCode = 401;
            //        return Task.CompletedTask;
            //    },
            //    OnRedirectToAccessDenied = redirectContext =>
            //        {
            //            redirectContext.HttpContext.Response.StatusCode = 401;
            //            return Task.CompletedTask;
            //        }
            //    };
            //});

            //jwt validation in middleware
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This-is-my-sceret-key"));
            var tokenValidationParameter = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = signingKey
            };

            services.AddAuthentication(x => x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(jwt => jwt.TokenValidationParameters = tokenValidationParameter);

            services.AddSwaggerDocument();
            services.AddControllers();
            services.AddDbContext<OrganizationDbContext>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //Gloablly handling Expection
            //app.UseExceptionHandler(options =>
            //{
            //    options.Run(async context =>
            //    {
            //        context.Response.StatusCode = 500;
            //        context.Response.ContentType = "application/json";
            //        var ex = context.Features.Get<IExceptionHandlerFeature>();
            //        if(ex != null)
            //        {
            //            await context.Response.WriteAsync(ex.Error.Message);
            //        }

            //    });
            //}
            //);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUi3();
            //app.UseCors("myPolicy");
            app.UseAuthentication();

        }
    }
}
