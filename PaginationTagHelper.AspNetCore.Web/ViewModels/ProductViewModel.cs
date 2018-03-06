using PaginationTagHelper.AspNetCore.Application.Entities;
using PaginationTagHelper.AspNetCore.Application.Models;

namespace PaginationTagHelper.AspNetCore.Web.ViewModels
{
    public class ProductViewModel
    {
        public PagedList<Product> Products { get; set; }
    }
}
