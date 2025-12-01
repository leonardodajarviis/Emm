namespace Emm.Application.Abstractions.Security;

public enum ResourceAction
{
    View = 0,
    Create = 1,
    Update = 2,
    Delete = 3,
    
    // Business Actions
    Approve = 10,
    Reject = 11,
    Assign = 12,
    StartProgress = 13,
    Resolve = 14,
    Close = 15
}
