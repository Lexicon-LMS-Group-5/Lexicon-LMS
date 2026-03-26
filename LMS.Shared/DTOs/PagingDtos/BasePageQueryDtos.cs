namespace LMS.Shared.DTOs.PagingDtos;

public class BasePageQueryDto
{
}

public class BasePagedResultDto<TItem>
{
    public IReadOnlyList<TItem> Items { get; }
}

public class PagedResultMetadataDto
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public SortOrder Order { get; set; }
}

public enum SortOrder
{
    Descending,
    Ascending,
}