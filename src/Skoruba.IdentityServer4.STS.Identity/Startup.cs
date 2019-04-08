﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SantillanaConnect.Domain.Entities.Users;
using SantillanaConnect.Domain.Model.DataContext;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.STS.Identity.Helpers;

namespace Skoruba.IdentityServer4.STS.Identity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public ILogger Logger { get; set; }

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
            Environment = environment;
            Logger = loggerFactory.CreateLogger<Startup>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureRootConfiguration(Configuration);

            // Add DbContext for Asp.Net Core Identity
            services.AddIdentityDbContext<MainContext>(Configuration, Environment);

            // Add email senders which is currently setup for SendGrid and SMTP
            services.AddEmailSenders(Configuration);

            // Add services for authentication, including Identity model, IdentityServer4 and external providers
            services.AddAuthenticationServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, MainContext, UserProfile, ApplicationRole>(Environment, Configuration, Logger);

            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddMvcWithLocalization<UserProfile, int>();

            // Add authorization policies for MVC
            services.AddAuthorizationPolicies();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.AddLogging(loggerFactory, Configuration);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvcLocalizationServices();
            app.UseMvcWithDefaultRoute();
        }
    }
}
