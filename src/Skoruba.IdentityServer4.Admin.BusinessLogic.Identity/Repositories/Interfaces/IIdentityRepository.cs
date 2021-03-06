﻿using Microsoft.AspNetCore.Identity;
using MR.AspNet.Identity.EntityFramework6;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.Dtos.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces
{
    public interface IIdentityRepository<TUserKey, TRoleKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<int, IdentityUserLoginInt, IdentityUserRoleInt, IdentityUserClaimInt, IdentityUserTokenInt>
        where TRole : IdentityRole<int, IdentityUserRoleInt, IdentityRoleClaimInt>
        where TUserClaim : IdentityUserClaimInt
        where TUserRole : IdentityUserRoleInt
        where TUserLogin : IdentityUserLoginInt
        where TRoleClaim : IdentityRoleClaimInt
        where TUserToken : IdentityUserTokenInt
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<PagedList<TUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<PagedList<TUser>> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10);

        Task<PagedList<TRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, int roleId)> CreateRoleAsync(TRole role);

        Task<TRole> GetRoleAsync(int roleId);

        Task<List<TRole>> GetRolesAsync();

        Task<(IdentityResult identityResult, int roleId)> UpdateRoleAsync(TRole role);

        Task<TUser> GetUserAsync(string userId);

        Task<(IdentityResult identityResult, int userId)> CreateUserAsync(TUser user);

        Task<(IdentityResult identityResult, int userId)> UpdateUserAsync(TUser user);

        Task<IdentityResult> DeleteUserAsync(string userId);

        Task<IdentityResult> CreateUserRoleAsync(string userId, string roleId);

        Task<PagedList<TRole>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(string userId, string roleId);

        Task<PagedList<TUserClaim>> GetUserClaimsAsync(string userId, int page = 1, int pageSize = 10);

        Task<TUserClaim> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaim claims);

        Task<int> DeleteUserClaimsAsync(string userId, int claimId);

        Task<List<UserLoginInfo>> GetUserProvidersAsync(string userId);

        Task<IdentityResult> DeleteUserProvidersAsync(string userId, string providerKey, string loginProvider);

        Task<TUserLogin> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(string userId, string password);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaim claims);

        Task<PagedList<TRoleClaim>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaim> GetRoleClaimAsync(string roleId, int claimId);

        Task<int> DeleteRoleClaimsAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleAsync(TRole role);
    }
}