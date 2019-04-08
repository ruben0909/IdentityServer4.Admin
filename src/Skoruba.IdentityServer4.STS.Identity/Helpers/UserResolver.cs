using SantillanaConnect.Authentication.Core.Identity.Identity;
using SantillanaConnect.Domain.Entities.Users;
using Skoruba.IdentityServer4.STS.Identity.Configuration;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class UserResolver
    {
        private readonly IdSrvUserManager _userManager;
        private readonly LoginResolutionPolicy _policy;

        public UserResolver(IdSrvUserManager userManager, LoginConfiguration configuration)
        {
            _userManager = userManager;
            _policy = configuration.ResolutionPolicy;
        }

        public async Task<UserProfile> GetUserAsync(string login)
        {
            switch (_policy)
            {
                case LoginResolutionPolicy.Username:
                    return await _userManager.FindByNameAsync(login);
                case LoginResolutionPolicy.Email:
                    return await _userManager.FindByEmailAsync(login);
                default:
                    return null;
            }
        }
    }
}
