using Microsoft.AspNetCore.Mvc;
using PaginationTagHelper.AspNetCore.Application.Services;
using PaginationTagHelper.AspNetCore.Web.Helpers;
using PaginationTagHelper.AspNetCore.Web.ViewModels;

namespace PaginationTagHelper.AspNetCore.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly IDataService _dataService;

        public HomeController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var vm = new ProductViewModel();
            vm.Products = _dataService.GetProductsPaged(page, pageSize);

            return View(vm);
        }

        public IActionResult Pager(int page = 1, int pageSize = 5)
        {
            var vm = new ProductViewModel();
            vm.Products = _dataService.GetProductsPaged(page, pageSize);

            return View("Index", vm);
        }

        public IActionResult AjaxGrid(int page = 1, int pageSize = 5)
        {
            var vm = new ProductViewModel();
            vm.Products = _dataService.GetProductsPaged(page, pageSize);

            return View(vm);
        }

        public IActionResult AjaxPager(int page = 1, int pageSize = 5)
        {
            var vm = new ProductViewModel();
            vm.Products = _dataService.GetProductsPaged(page, pageSize);

            if (HttpContext.Request.IsAjaxRequest())
                return PartialView("_AjaxPagedListPartialView", vm);

            return View("AjaxGrid", vm);
        }
    }
}
