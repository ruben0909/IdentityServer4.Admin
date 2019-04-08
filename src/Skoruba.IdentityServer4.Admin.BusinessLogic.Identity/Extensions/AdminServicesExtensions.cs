using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MR.AspNet.Identity.EntityFramework6;
using SantillanaConnect.Domain.Model.Extensions;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Interfaces;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AdminServicesExtensions
    {
        private class MapperConfigurationBuilder : IMapperConfigurationBuilder
        {
            public HashSet<Type> ProfileTypes { get; } = new HashSet<Type>();

            public IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey,
                TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
                TUserClaimDto, TRoleClaimDto>()
                where TUserDto : UserDto<TUserDtoKey>
                where TRoleDto : RoleDto<TRoleDtoKey>
                where TUser : IdentityUser<int, IdentityUserLoginInt, IdentityUserRoleInt, IdentityUserClaimInt, IdentityUserTokenInt>
                where TRole : IdentityRole<int, IdentityUserRoleInt, IdentityRoleClaimInt>
                where TKey : IEquatable<TKey>
                where TUserClaim : IdentityUserClaimInt
                where TUserRole : IdentityUserRoleInt
                where TUserLogin : IdentityUserLoginInt
                where TRoleClaim : IdentityRoleClaimInt
                where TUserToken : IdentityUserTokenInt
                where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
                where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
                where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
                where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
                where TUserProviderDto : UserProviderDto<TUserDtoKey>
                where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
                where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
                where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
                where TUserClaimDto : UserClaimDto<TUserDtoKey>
                where TRoleClaimDto : RoleClaimDto<TRoleDtoKey>
            {
                ProfileTypes.Add(typeof(IdentityMapperProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey,
                    TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>));

                return this;
            }
        }

        public static IMapperConfigurationBuilder AddAdminAspNetIdentityMapping(this IServiceCollection services)
        {
            var builder = new MapperConfigurationBuilder();

            services.AddSingleton<IConfigurationProvider>(sp => new MapperConfiguration(cfg =>
            {
                foreach (var profileType in builder.ProfileTypes)
                {
                    cfg.AddProfile(profileType);
                }
            }));

            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

            return builder;
        }
        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole>(
            this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TUser : IdentityUserInt
            where TRole : IdentityRoleInt
            where TIdentityDbContext : IdentityDbContextInt<TUser, TRole>
        {
            return services.AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, UserDto<int>, int, RoleDto<int>, int, int, int, TUser,
                TRole,
                int, IdentityUserClaimInt, IdentityUserRoleInt, IdentityUserLoginInt, IdentityRoleClaimInt, IdentityUserTokenInt,
                UsersDto<UserDto<int>, int>, RolesDto<RoleDto<int>, int>, UserRolesDto<RoleDto<int>, int, int>, UserClaimsDto<int>, UserProviderDto<int>,
                UserProvidersDto<int>, UserChangePasswordDto<int>, RoleClaimsDto<int>, UserClaimDto<int>, RoleClaimDto<int>>();
        }
        //Commmented because needs 2 context of core and identity is not for SC
        //public static IServiceCollection AddAdminAspNetIdentityServices<TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey,
        //    TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        //    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        //    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
        //    TUserClaimDto, TRoleClaimDto>(
        //        this IServiceCollection services)
        //    where TAdminDbContext :
        //    IdentityDbContext<TUser, TRole, TKey, IdentityUserLoginInt, IdentityUserRoleInt, IdentityUserClaimInt, IdentityRoleClaimInt, IdentityUserTokenInt>,
        //    IAdminPersistedGrantDbContext
        //    where TUserDto : UserDto<TUserDtoKey>
        //    where TUser : IdentityUser<int, IdentityUserLoginInt, IdentityUserRoleInt, IdentityUserClaimInt, IdentityUserTokenInt>
        //    where TRole : IdentityRole<int, IdentityUserRoleInt, IdentityRoleClaimInt>
        //    where TKey : IEquatable<TKey>
        //    where TUserClaim : IdentityUserClaimInt
        //    where TUserRole : IdentityUserRoleInt
        //    where TUserLogin : IdentityUserLoginInt
        //    where TRoleClaim : IdentityRoleClaimInt
        //    where TUserToken : IdentityUserTokenInt
        //    where TRoleDto : RoleDto<TRoleDtoKey>
        //    where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
        //    where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
        //    where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
        //    where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
        //    where TUserProviderDto : UserProviderDto<TUserDtoKey>
        //    where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
        //    where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
        //    where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
        //    where TUserClaimDto : UserClaimDto<TUserDtoKey>
        //    where TRoleClaimDto : RoleClaimDto<TRoleDtoKey>
        //{

        //    return services.AddAdminAspNetIdentityServices<TAdminDbContext, TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey,
        //        TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        //        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        //        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>();
        //}

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
                        this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, int, IdentityUserLoginInt, IdentityUserRoleInt, IdentityUserClaimInt, IdentityRoleClaimInt, IdentityUserTokenInt>
            where TUserDto : UserDto<TUserDtoKey>
            where TUser : IdentityUser<int, IdentityUserLoginInt, IdentityUserRoleInt, IdentityUserClaimInt, IdentityUserTokenInt>
            where TRole : IdentityRole<int, IdentityUserRoleInt, IdentityRoleClaimInt>
            where TKey : IEquatable<TKey>
            where TUserClaim : IdentityUserClaimInt
            where TUserRole : IdentityUserRoleInt
            where TUserLogin : IdentityUserLoginInt
            where TRoleClaim : IdentityRoleClaimInt
            where TUserToken : IdentityUserTokenInt
            where TRoleDto : RoleDto<TRoleDtoKey>
            where TUsersDto : UsersDto<TUserDto, TUserDtoKey>
            where TRolesDto : RolesDto<TRoleDto, TRoleDtoKey>
            where TUserRolesDto : UserRolesDto<TRoleDto, TUserDtoKey, TRoleDtoKey>
            where TUserClaimsDto : UserClaimsDto<TUserDtoKey>
            where TUserProviderDto : UserProviderDto<TUserDtoKey>
            where TUserProvidersDto : UserProvidersDto<TUserDtoKey>
            where TUserChangePasswordDto : UserChangePasswordDto<TUserDtoKey>
            where TRoleClaimsDto : RoleClaimsDto<TRoleDtoKey>
            where TUserClaimDto : UserClaimDto<TUserDtoKey>
            where TRoleClaimDto : RoleClaimDto<TRoleDtoKey>
        {
            //Repositories
            services.AddTransient<IIdentityRepository<TUserKey, TRoleKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityRepository<TIdentityDbContext, TUserKey, TRoleKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<IPersistedGrantAspNetIdentityRepository, PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();

            //Services
            services.AddTransient<IIdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>,
                IdentityService<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUserKey, TRoleKey, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto>>();
            services.AddTransient<IPersistedGrantAspNetIdentityService, PersistedGrantAspNetIdentityService>();

            //Resources
            services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
            services.AddScoped<IPersistedGrantAspNetIdentityServiceResources, PersistedGrantAspNetIdentityServiceResources>();

            //Register mapping
            services.AddAdminAspNetIdentityMapping()
                    .UseIdentityMappingProfile<TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>();

            return services;
        }
    }
}
