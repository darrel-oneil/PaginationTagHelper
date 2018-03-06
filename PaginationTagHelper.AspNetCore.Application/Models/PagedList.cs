using System;
using System.Collections.Generic;

namespace PaginationTagHelper.AspNetCore.Application.Models
{
    public class PagedList<T> : IPagedList<T>
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalItemCount { get; set; }

        public IList<T> Results { get; set; }

        public int PageCount
        {
            get
            {
                if (TotalItemCount > 0 && PageSize > 0)
                    return (int)Math.Ceiling(TotalItemCount / (double)PageSize);
                return 0;
            }
        }
    }
}
