using Emm.Application.Abstractions;
using Emm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Emm.Infrastructure.Services;

/// <summary>
/// Authorization service implementation with caching
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUserPermissionRepository _userPermissionRepository;
    private readonly IPolicyRepository _policyRepository;
    private readonly IPolicyEvaluator _policyEvaluator;
    private readonly IMemoryCache _cache;

    private const int CacheExpirationMinutes = 15;

    public AuthorizationService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IUserRoleRepository userRoleRepository,
        IUserPermissionRepository userPermissionRepository,
        IPolicyRepository policyRepository,
        IPolicyEvaluator policyEvaluator,
        IMemoryCache cache)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _userRoleRepository = userRoleRepository;
        _userPermissionRepository = userPermissionRepository;
        _policyRepository = policyRepository;
        _policyEvaluator = policyEvaluator;
        _cache = cache;
    }

    public async Task<bool> HasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default)
    {
        var permissions = await GetUserPermissionsInternalAsync(userId, cancellationToken);
        return permissions.Contains(permissionCode);
    }

    public async Task<bool> HasAnyPermissionAsync(Guid userId, IEnumerable<string> permissionCodes, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsInternalAsync(userId, cancellationToken);
        return permissionCodes.Any(code => userPermissions.Contains(code));
    }

    public async Task<bool> HasAllPermissionsAsync(Guid userId, IEnumerable<string> permissionCodes, CancellationToken cancellationToken = default)
    {
        var userPermissions = await GetUserPermissionsInternalAsync(userId, cancellationToken);
        return permissionCodes.All(code => userPermissions.Contains(code));
    }

    public async Task<bool> CanAccessAsync(
        Guid userId,
        string resource,
        string action,
        object? resourceContext = null,
        CancellationToken cancellationToken = default)
    {
        // 1. Check basic permission
        var permissionCode = $"{resource}.{action}";
        var hasPermission = await HasPermissionAsync(userId, permissionCode, cancellationToken);

        if (!hasPermission)
            return false;

        // 2. Evaluate ABAC policies
        var policies = await _policyRepository.GetByResourceTypeAsync(resource, cancellationToken);

        if (!policies.Any())
            return true; // No policies = allow

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
            return false;

        var context = new PolicyContext
        {
            UserId = userId,
            UserOrganizationUnitId = user.OrganizationUnitId,
            Resource = resource,
            Action = action,
            RequestTime = DateTime.UtcNow,
            ResourceAttributes = ExtractResourceAttributes(resourceContext)
        };

        return await _policyEvaluator.EvaluatePoliciesAsync(policies, context, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return [.. await GetUserPermissionsInternalAsync(userId, cancellationToken)];
    }

    public async Task<IReadOnlyList<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_roles_{userId}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes);

            var roles = await _roleRepository.GetByUserIdAsync(userId, cancellationToken);
            return roles.Select(r => r.Code).ToList();
        }) ?? new List<string>();
    }

    private async Task<HashSet<string>> GetUserPermissionsInternalAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"user_permissions_{userId}";

        return await _cache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CacheExpirationMinutes);

            var permissions = new HashSet<string>();

            // 1. Get permissions from roles
            var userRoles = await _userRoleRepository.GetByUserIdAsync(userId, cancellationToken);
            foreach (var userRole in userRoles)
            {
                var role = await _roleRepository.GetByIdWithPermissionsAsync(userRole.RoleId, cancellationToken);
                if (role != null)
                {
                    foreach (var rp in role.RolePermissions)
                    {
                        permissions.Add(rp.Permission.Code);
                    }
                }
            }

            // 2. Get direct user permissions
            var userPermissions = await _userPermissionRepository.GetByUserIdAsync(userId, cancellationToken);
            foreach (var up in userPermissions)
            {
                if (up.IsGranted)
                {
                    permissions.Add(up.Permission.Code);
                }
                else
                {
                    // Deny takes precedence - remove if exists
                    permissions.Remove(up.Permission.Code);
                }
            }

            return permissions;
        }) ?? new HashSet<string>();
    }

    private Dictionary<string, object> ExtractResourceAttributes(object? resourceContext)
    {
        if (resourceContext == null)
            return new Dictionary<string, object>();

        var attributes = new Dictionary<string, object>();
        var properties = resourceContext.GetType().GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(resourceContext);
            if (value != null)
            {
                attributes[prop.Name] = value;
            }
        }

        return attributes;
    }
}
