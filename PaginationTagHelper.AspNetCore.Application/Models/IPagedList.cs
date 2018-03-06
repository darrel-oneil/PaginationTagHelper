using System.Collections.Generic;

namespace PaginationTagHelper.AspNetCore.Application.Models
{
    public interface IPagedList<T>
    {
        int CurrentPage { get; set; }

        int PageSize { get; set; }

        int TotalItemCount { get; set; }

        IList<T> Results { get; set; }

        int PageCount { get; }
    }
}
