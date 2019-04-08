using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using SantillanaConnect.Domain.Entities.Users;
using SantillanaConnect.Domain.Model.DataContext;
using Serilog;
using Skoruba.IdentityServer4.Admin.EntityFramework.DbContexts;
using Skoruba.IdentityServer4.Admin.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin
{
    public class Program
    {
        private const string SeedArgs = "/seed";

        public static async Task Main(string[] args)
        {
            args = new string[] { "/seed" };
            var seed = args.Any(x => x == SeedArgs);
            if (seed)
            {
                args = args.Except(new[] { SeedArgs }).ToArray();
            }

            var host = BuildWebHost(args);

            // Uncomment this to seed upon startup, alternatively pass in `dotnet run /seed` to seed using CLI
            // await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, AdminIdentityDbContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, UserProfile, ApplicationRole>(host);
            if (seed)
            {
                await DbMigrationHelpers.EnsureSeedData<IdentityServerConfigurationDbContext, MainContext, IdentityServerPersistedGrantDbContext, AdminLogDbContext, UserProfile, ApplicationRole>(host.Services);
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseKestrel(c => c.AddServerHeader = false)
                   .UseStartup<Startup>()
                   .UseSerilog()
                   .Build();
    }
}