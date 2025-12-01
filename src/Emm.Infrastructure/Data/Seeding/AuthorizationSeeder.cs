using Emm.Domain.Entities.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Emm.Infrastructure.Data.Seeding;

public static class AuthorizationSeeder
{
    public static async Task SeedAuthorizationDataAsync(XDbContext context)
    {
        // Check if already seeded
        if (await context.Set<Permission>().AnyAsync())
            return;

        // 1. Create Permissions
        var permissions = new List<Permission>
        {
            // Permission Management
            new("Permission", "View", "Xem danh sách quyền", "Cho phép xem danh sách permissions", "Authorization"),
            new("Permission", "Create", "Tạo quyền mới", "Cho phép tạo permission mới", "Authorization"),
            new("Permission", "Update", "Cập nhật quyền", "Cho phép chỉnh sửa permission", "Authorization"),
            new("Permission", "Delete", "Xóa quyền", "Cho phép xóa permission", "Authorization"),
            new("Permission", "Check", "Kiểm tra quyền", "Cho phép check permission của user", "Authorization"),

            // Role Management
            new("Role", "View", "Xem danh sách vai trò", "Cho phép xem danh sách roles", "Authorization"),
            new("Role", "Create", "Tạo vai trò mới", "Cho phép tạo role mới", "Authorization"),
            new("Role", "Update", "Cập nhật vai trò", "Cho phép chỉnh sửa role", "Authorization"),
            new("Role", "Delete", "Xóa vai trò", "Cho phép xóa role", "Authorization"),

            // User Management
            new("User", "View", "Xem người dùng", "Cho phép xem thông tin users", "User Management"),
            new("User", "Create", "Tạo người dùng", "Cho phép tạo user mới", "User Management"),
            new("User", "Update", "Cập nhật người dùng", "Cho phép chỉnh sửa user", "User Management"),
            new("User", "Delete", "Xóa người dùng", "Cho phép xóa user", "User Management"),
            new("User", "AssignRole", "Gán vai trò", "Cho phép gán role cho user", "User Management"),
            new("User", "AssignPermission", "Gán quyền", "Cho phép gán permission cho user", "User Management"),
            new("User", "ViewPermissions", "Xem quyền người dùng", "Cho phép xem permissions của user", "User Management"),

            // Operation Shift Management
            new("OperationShift", "View", "Xem ca vận hành", "Cho phép xem operation shifts", "Operations"),
            new("OperationShift", "Create", "Tạo ca vận hành", "Cho phép tạo operation shift", "Operations"),
            new("OperationShift", "Update", "Cập nhật ca vận hành", "Cho phép chỉnh sửa operation shift", "Operations"),
            new("OperationShift", "Delete", "Xóa ca vận hành", "Cho phép xóa operation shift", "Operations"),
            new("OperationShift", "Start", "Bắt đầu ca", "Cho phép start shift", "Operations"),
            new("OperationShift", "Complete", "Hoàn thành ca", "Cho phép complete shift", "Operations"),
            new("OperationShift", "Cancel", "Hủy ca", "Cho phép cancel shift", "Operations"),

            // Asset Management
            new("Asset", "View", "Xem tài sản", "Cho phép xem assets", "Assets"),
            new("Asset", "Create", "Tạo tài sản", "Cho phép tạo asset", "Assets"),
            new("Asset", "Update", "Cập nhật tài sản", "Cho phép chỉnh sửa asset", "Assets"),
            new("Asset", "Delete", "Xóa tài sản", "Cho phép xóa asset", "Assets"),

            // Report
            new("Report", "View", "Xem báo cáo", "Cho phép xem reports", "Reports"),
            new("Report", "Export", "Xuất báo cáo", "Cho phép export reports", "Reports"),
        };

        await context.Set<Permission>().AddRangeAsync(permissions);
        await context.SaveChangesAsync();

        // 2. Create Roles
        var adminRole = new Role("ADMIN", "Quản trị viên hệ thống", "Có toàn quyền trên hệ thống", isSystemRole: true);
        var managerRole = new Role("MANAGER", "Quản lý", "Quản lý vận hành và nhân sự", isSystemRole: false);
        var supervisorRole = new Role("SHIFT_SUPERVISOR", "Giám sát ca", "Giám sát và quản lý ca vận hành", isSystemRole: false);
        var operatorRole = new Role("OPERATOR", "Vận hành viên", "Thực hiện vận hành thiết bị", isSystemRole: false);
        var viewerRole = new Role("VIEWER", "Người xem", "Chỉ được xem thông tin", isSystemRole: false);

        await context.Set<Role>().AddRangeAsync(new[] { adminRole, managerRole, supervisorRole, operatorRole, viewerRole });
        await context.SaveChangesAsync();

        // 3. Assign Permissions to Roles
        // Admin - ALL permissions
        foreach (var permission in permissions)
        {
            adminRole.AddPermission(permission.Id);
        }

        // Manager - Most permissions except system management
        var managerPermissions = permissions.Where(p =>
            p.Resource != "Permission" &&
            p.Resource != "Role").ToList();
        foreach (var permission in managerPermissions)
        {
            managerRole.AddPermission(permission.Id);
        }

        // Shift Supervisor - Operation shifts and assets
        var supervisorPermissions = permissions.Where(p =>
            p.Resource == "OperationShift" ||
            p.Resource == "Asset" ||
            (p.Resource == "Report" && p.Action == "View")).ToList();
        foreach (var permission in supervisorPermissions)
        {
            supervisorRole.AddPermission(permission.Id);
        }

        // Operator - Basic operation permissions
        var operatorPermissions = permissions.Where(p =>
            (p.Resource == "OperationShift" && (p.Action == "View" || p.Action == "Start" || p.Action == "Complete")) ||
            (p.Resource == "Asset" && p.Action == "View")).ToList();
        foreach (var permission in operatorPermissions)
        {
            operatorRole.AddPermission(permission.Id);
        }

        // Viewer - Only view permissions
        var viewerPermissions = permissions.Where(p => p.Action == "View").ToList();
        foreach (var permission in viewerPermissions)
        {
            viewerRole.AddPermission(permission.Id);
        }

        context.Set<Role>().UpdateRange(adminRole, managerRole, supervisorRole, operatorRole, viewerRole);
        await context.SaveChangesAsync();

        // 4. Create sample ABAC Policy - Organization Unit Restriction
        var orgUnitPolicy = new Policy(
            "ORG_UNIT_RESTRICTION",
            "Giới hạn theo đơn vị tổ chức",
            PolicyType.OrganizationUnit,
            "OperationShift",
            null,
            "Người dùng chỉ có thể truy cập operation shifts thuộc đơn vị của mình",
            priority: 100);

        orgUnitPolicy.SetConditions(new
        {
            RequireSameOrganizationUnit = true
        });

        await context.Set<Policy>().AddAsync(orgUnitPolicy);
        await context.SaveChangesAsync();
    }
}
