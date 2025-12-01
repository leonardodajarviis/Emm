namespace Emm.Application.Common;

public class PagedResult
{
    public PagedResult(int page, int pageSize, int totalCount, IReadOnlyList<object> results)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Results = results;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool IsNextPageAvailable => Page < TotalPages;
    public IReadOnlyList<object> Results { get; } = [];
}


