using Gridify;

namespace Emm.Application.Common;

public class QueryParam : IGridifyQuery
{
    public PagedResult AsPagedResult(int totalCount = 0, IReadOnlyList<object>? items = null)
    {
        return new PagedResult(Page, PageSize, totalCount, items ?? []);
    }

    public string? SearchTerm { get; set; }
    public int Page {get; set;}
    public int PageSize {get; set;}
    public string? Filter {get; set;}
    public string? OrderBy {get; set;}


    public string? GetSearch()
    {
        return string.IsNullOrEmpty(SearchTerm) ? null : SearchTerm.Trim();
    }

}
