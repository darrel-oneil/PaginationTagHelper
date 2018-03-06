using PaginationTagHelper.AspNetCore.Application.Entities;
using PaginationTagHelper.AspNetCore.Application.Models;

namespace PaginationTagHelper.AspNetCore.Application.Services
{
    public interface IDataService
    {
        PagedList<Product> GetProductsPaged(int page = 1, int pageSize = 20);
    }
}
