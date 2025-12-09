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
            ResourceAction.Close => CheckClosePermission(resource, userId.Value),
            ResourceAction.View => Task.FromResult(true), // Ai cũng xem được (tạm thời)
            _ => Task.FromResult(false)
        };
    }

    private Task<bool> CheckUpdatePermission(IncidentReport report, long userId)
    {
        // Người tạo được sửa khi còn mới
        if (report.Audit.CreatedByUserId == userId && report.Status == IncidentStatus.New)
        {
            return Task.FromResult(true);
        }

        // TODO: Manager của Asset location cũng được sửa

        return Task.FromResult(false);
    }

    private Task<bool> CheckDeletePermission(IncidentReport report, long userId)
    {
        // Chỉ người tạo được xóa khi còn mới
        return Task.FromResult(report.Audit.CreatedByUserId == userId && report.Status == IncidentStatus.New);
    }

    private Task<bool> CheckClosePermission(IncidentReport report, long userId)
    {
        // Người tạo confirm đóng phiếu sau khi đã resolve
        return Task.FromResult(report.Audit.CreatedByUserId == userId && report.Status == IncidentStatus.Resolved);
    }
}
