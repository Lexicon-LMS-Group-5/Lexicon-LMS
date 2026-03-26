using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.PagingDtos;

public class BasePageQueryDto
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(2, 25)]
    public int Size { get; set; } = 2;

    public SortOrder? Order { get; set; } = SortOrder.Descending;
}

public class BasePagedResultDto<TItem>
{
    public IReadOnlyList<TItem> Items { get; }
    public PagedResultMetaDataDto MetaData { get; set; }
}

public class PagedResultMetaDataDto
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