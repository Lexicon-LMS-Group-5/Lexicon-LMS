using System.ComponentModel.DataAnnotations;

namespace LMS.Shared.DTOs.PagingDtos;

public class BasePageQueryDto
{
    const int MinPageSize = 2;
    const int MaxPageSize = 50;
    private int offset { get => Page - 1; }
    private int size { get => Size > MaxPageSize || Size < MinPageSize ? MaxPageSize : Size; }
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(MinPageSize, MaxPageSize)]
    public int Size { get; set; } = 25;

    public int Skip { get => size * offset; }

    public SortOrder? Order { get; set; } = SortOrder.Descending;
}

public class BasePagedResultDto<TItem>
{
    public IReadOnlyList<TItem> Items { get; init; } = [];
    public PagedResultMetaDataDto MetaData { get; set; } = default!;
}

public class PagedResultMetaDataDto
{
    public int Page { get; set; }
    public int Size { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public string Order { get; set; } = string.Empty;
}

public enum SortOrder
{
    Descending,
    Ascending,
}