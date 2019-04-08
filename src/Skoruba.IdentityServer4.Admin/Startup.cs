

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MR.AspNet.Identity.EntityFramework6;
using SantillanaConnect.Domain.Entities.Users;
using SantillanaConnect.Domain.Model.DataContext;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.Configuration.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.Helpers;
using System.IdentityModel.Tokens.Jwt;

namespace Skoruba.IdentityServer4.Admin
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var builder = new ConfigurationBuilder()
                    .SetBasePath(env.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();

            HostingEnvironment = env;
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Get Configuration
            services.ConfigureRootConfiguration(Configuration);
            var rootConfiguration = services.BuildServiceProvider().GetService<IRootConfiguration>();

            // Add DbContexts for Asp.Net Core Identity, Logging and IdentityServer - Configuration store and Operational store
            services.AddDbContexts<MainContext, IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>(HostingEnvironment, Configuration);

            // Add Asp.Net Core Identity Configuration and OpenIdConnect auth as well
            services.AddAuthenticationServices<MainContext, UserProfile, ApplicationRole>(HostingEnvironment, rootConfiguration.AdminConfiguration);

            // Add exception filters in MVC
            services.AddMvcExceptionFilters();

            // Add all dependencies for IdentityServer Admin
            services.AddAdminServices<IdentityServerConfigurationDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext>();

            // Add all dependencies for Asp.Net Core Identity
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddAdminAspNetIdentityServices<MainContext, IdentityServerPersistedGrantDbContext, UserDto<int>, int, RoleDto<int>, int, int, int,
                                UserProfile, ApplicationRole, int, IdentityUserClaimInt, IdentityUserRoleInt,
                                IdentityUserLoginInt, IdentityRoleClaimInt, IdentityUserTokenInt,
                                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                                UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                                RoleClaimsDto<int>, UserClaimDto<int>, RoleClaimDto<int>>();

            // Add all dependencies for Asp.Net Core Identity in MVC - these dependencies are injected into generic Controllers
            // Including settings for MVC and Localization
            // If you want to change primary keys or use another db model for Asp.Net Core Identity:
            services.AddMvcWithLocalization<UserDto<int>, int, RoleDto<int>, int, int, int,
                UserProfile, ApplicationRole, int, IdentityUserClaimInt, IdentityUserRoleInt,
                IdentityUserLoginInt, IdentityRoleClaimInt, IdentityUserTokenInt,
                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>,
                UserClaimsDto<int>, UserProviderDto<int>, UserProvidersDto<int>, UserChangePasswordDto<int>,
                RoleClaimsDto<int>>();

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
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Add custom security headers
            app.UseSecurityHeaders();

            app.UseStaticFiles();

            // Use authentication and for integration tests use custom middleware which is used only in Staging environment
            app.ConfigureAuthenticationServices(env);

            // Use Localization
            app.ConfigureLocalization();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}