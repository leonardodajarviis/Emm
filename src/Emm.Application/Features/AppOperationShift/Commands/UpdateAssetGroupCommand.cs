using Emm.Application.Common;
using Emm.Domain.Entities.Operations;

namespace Emm.Application.Features.AppOperationShift.Commands;

public record UpdateAssetGroupCommand(
    long OperationShiftId,
    long AssetGroupId,
    string GroupName,
    GroupRole Role,
    int DisplayOrder,
    string? Description) : IRequest<Result<object>>;
