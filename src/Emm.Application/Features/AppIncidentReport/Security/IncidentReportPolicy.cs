using Emm.Application.Abstractions;
using Emm.Application.Abstractions.Security;
using Emm.Domain.Entities.Maintenance;

namespace Emm.Application.Features.AppIncidentReport.Security;

public class IncidentReportPolicy : IResourcePolicy<IncidentReport>
{
    private readonly IUserContextService _userContext;

    public IncidentReportPolicy(IUserContextService userContext)
    {
        _userContext = userContext;
    }

    public Task<bool> CheckAsync(IncidentReport resource, ResourceAction action, CancellationToken cancellationToken)
    {
        var userId = _userContext.GetCurrentUserId();
        if (userId == null) return Task.FromResult(false);

        // TODO: Check User Role (e.g., if Admin -> return true)
        // var roles = _userContext.GetCurrentUserRoles();
        // if (roles.Contains("Admin")) return true;

        return action switch
        {
            ResourceAction.Update => CheckUpdatePermission(resource, userId.Value),
            ResourceAction.Delete => CheckDeletePermission(resource, userId.Value),
            ResourceAction.Resolve => CheckResolvePermission(resource, userId.Value),
            ResourceAction.Close => CheckClosePermission(resource, userId.Value),
            ResourceAction.Assign => CheckAssignPermission(resource, userId.Value),
            ResourceAction.StartProgress => CheckStartProgressPermission(resource, userId.Value),
            ResourceAction.View => Task.FromResult(true), // Ai cũng xem được (tạm thời)
            _ => Task.FromResult(false)
        };
    }

    private Task<bool> CheckUpdatePermission(IncidentReport report, long userId)
    {
        // Người tạo được sửa khi còn mới
        if (report.CreatedByUserId == userId && report.Status == IncidentStatus.New)
        {
            return Task.FromResult(true);
        }

        // TODO: Manager của Asset location cũng được sửa

        return Task.FromResult(false);
    }

    private Task<bool> CheckDeletePermission(IncidentReport report, long userId)
    {
        // Chỉ người tạo được xóa khi còn mới
        return Task.FromResult(report.CreatedByUserId == userId && report.Status == IncidentStatus.New);
    }

    private Task<bool> CheckResolvePermission(IncidentReport report, long userId)
    {
        // Chỉ người được assign hoặc người tạo (demo) mới được resolve
        // Thực tế nên check xem user có thuộc đội bảo trì không
        if (report.Status == IncidentStatus.InProgress || report.Status == IncidentStatus.Assigned)
        {
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private Task<bool> CheckClosePermission(IncidentReport report, long userId)
    {
        // Người tạo confirm đóng phiếu sau khi đã resolve
        return Task.FromResult(report.CreatedByUserId == userId && report.Status == IncidentStatus.Resolved);
    }

    private Task<bool> CheckAssignPermission(IncidentReport report, long userId)
    {
        // Chỉ manager mới được assign (Logic demo: cho phép tất cả user login tạm thời)
        return Task.FromResult(report.Status == IncidentStatus.New || report.Status == IncidentStatus.Assigned);
    }

    private Task<bool> CheckStartProgressPermission(IncidentReport report, long userId)
    {
        return Task.FromResult(report.Status == IncidentStatus.Assigned);
    }
}
