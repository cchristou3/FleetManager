using System;
using System.Collections.Generic;

namespace Fleet.Api.Shared.Pagination;

public class PaginationResponse<TDestination> : List<TDestination>
    where TDestination : class
{
    private PaginationResponse(List<TDestination> dest, int count, PaginationParams paginationParams)
    {
        CurrentPage = paginationParams.PageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)paginationParams.PageSize);
        PageSize = paginationParams.PageSize;
        TotalCount = count;
        AddRange(dest);
    }


    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public static PaginationResponse<TDestination> Create(List<TDestination> dest, int count,
        PaginationParams paginationParams)
    {
        return new PaginationResponse<TDestination>(dest, count, paginationParams);
    }
}