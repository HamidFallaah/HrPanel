namespace HrPanel.Application.Common.Models;

public sealed record PagedResult<TItem>
{
    private PagedResult(IReadOnlyCollection<TItem> items,int pageNumber,int pageSize,int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;

        TotalPages = totalCount == 0? 0: (int)Math.Ceiling(totalCount / (double)pageSize);
    }
    public IReadOnlyCollection<TItem> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public static PagedResult<TItem> Create(IReadOnlyCollection<TItem> items,int pageNumber,int pageSize,int totalCount)
    {
        return new PagedResult<TItem>(items,pageNumber,pageSize,totalCount);
    }
}