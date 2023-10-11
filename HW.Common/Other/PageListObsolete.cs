using HW.Common.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace HW.Common.Other; 

public class PagedListObsolete<T>
{
    public List<T> Items { get; private set; }
    public Pagination Pagination { get; set; } = new Pagination();
   

    public PagedListObsolete(List<T> items, int count, int pageNumber, int pageSize)
    {
        Pagination.TotalCount = count;
        Pagination.PageSize = pageSize;
        Pagination.CurrentPage = pageNumber;
        Pagination.HasPrevious = pageNumber > 1;
        Pagination.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        Pagination.HasNext = pageNumber < (int)Math.Ceiling(count / (double)pageSize);
        Items = items;
    }

    public static Task<PagedList<T>> ToPagedList(List<T> source, int pageNumber, int pageSize)
    {
        var count =   source.Count;
        var items =   source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return Task.FromResult(new PagedList<T>(items, count, pageNumber, pageSize));
    }
}