using Bogus;
using PaginationTagHelper.AspNetCore.Application.Entities;
using PaginationTagHelper.AspNetCore.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaginationTagHelper.AspNetCore.Application.Services
{
    public class InMemoryDataService : IDataService
    {
        static IList<Product> _dataSource;

        public InMemoryDataService()
        {
            var faker = new Faker("en");
            _dataSource = new List<Product>();
            InitialiseTestData(500);
        }

        private void InitialiseTestData(int maxRows)
        {
            for (int i = 1; i <= maxRows; i++)
            {
                var product = new Bogus.DataSets.Commerce();

                _dataSource.Add(new Product
                {
                    Id = i,
                    Name = product.ProductName(),
                    Price = Convert.ToDouble(product.Price()),
                    SKU = product.Random.AlphaNumeric(8).ToUpper()
                });
            }
        }

        public PagedList<Product> GetProductsPaged(int page = 1, int pageSize = 20)
        {
            page = (page < 1) ? 1 : page;
            pageSize = (pageSize < 1) ? 20 : pageSize;

            var pagedList = new PagedList<Product>();
            pagedList.TotalItemCount = _dataSource.Count();
            pagedList.PageSize = pageSize;
            pagedList.CurrentPage = (page > pagedList.PageCount) ? 1 : page;

            var skip = (pagedList.CurrentPage - 1) * pagedList.PageSize;

            pagedList.Results = _dataSource.Skip(skip)
                                .Take(pageSize)
                                .ToList();

            return pagedList;
        }
    }
}
