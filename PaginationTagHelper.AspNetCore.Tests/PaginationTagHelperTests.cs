using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PaginationTagHelper.AspNetCore.Tests
{
    public class PaginationTagHelperTests : IDisposable
    {
        ViewContext viewContext;
        Mock<IHttpContextAccessor> context;
        Mock<HttpRequest> request;

        public PaginationTagHelperTests()
        {
            viewContext = new ViewContext();
            context = new Mock<IHttpContextAccessor>();
            request = new Mock<HttpRequest>();
        }

        public void Dispose()
        {
            viewContext = null;
            context = null;
            request = null;
        }

        [Fact(DisplayName = "Process [on initial request] generates default output")]
        public void Process_OnInitialRequest_GeneratesDefaultOutput()
        {
            // Arrange
            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.TotalItems = 50;
            paginationTagHelper.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery("");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml = "<li class=\"disabled\"><a>&#171;</a></li>" +
                "<li class=\"disabled\"><a>&#8249; Previous</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li class=\"active\"><a href=\"/home/pager?page=1&pageSize=10\">1</a></li>" +
                "<li><a href=\"/home/pager?page=2&pageSize=10\">2</a></li>" +
                "<li><a href=\"/home/pager?page=3&pageSize=10\">3</a></li>" +
                "<li><a href=\"/home/pager?page=4&pageSize=10\">4</a></li>" +
                "<li><a href=\"/home/pager?page=5&pageSize=10\">5</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li><a href=\"/home/pager?page=2&pageSize=10\">Next &#8250;</a></li>" +
                "<li><a href=\"/home/pager?page=5&pageSize=10\">&#187;</a></li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName="Process [when navigating to first page] generates expected output")]
        public void Process_OnFirstPage_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.Page = 1;
            paginationTagHelper.PageSize = 10;
            paginationTagHelper.TotalItems = 50;
            paginationTagHelper.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={paginationTagHelper.Page}&pageSize={paginationTagHelper.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml = "<li class=\"disabled\"><a>&#171;</a></li>" +
                "<li class=\"disabled\"><a>&#8249; Previous</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li class=\"active\"><a href=\"/home/pager?page=1&pageSize=10\">1</a></li>" +
                "<li><a href=\"/home/pager?page=2&pageSize=10\">2</a></li>" +
                "<li><a href=\"/home/pager?page=3&pageSize=10\">3</a></li>" +
                "<li><a href=\"/home/pager?page=4&pageSize=10\">4</a></li>" +
                "<li><a href=\"/home/pager?page=5&pageSize=10\">5</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li><a href=\"/home/pager?page=2&pageSize=10\">Next &#8250;</a></li>" +
                "<li><a href=\"/home/pager?page=5&pageSize=10\">&#187;</a></li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when navigating to last page] generates expected output")]
        public void Process_OnLastPage_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.Page = 5;
            paginationTagHelper.PageSize = 10;
            paginationTagHelper.TotalItems = 50;
            paginationTagHelper.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={paginationTagHelper.Page}&pageSize={paginationTagHelper.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                "<li><a href=\"/home/pager?page=1&pageSize=10\">&#171;</a></li>" +
                "<li><a href=\"/home/pager?page=4&pageSize=10\">&#8249; Previous</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li><a href=\"/home/pager?page=1&pageSize=10\">1</a></li>" +
                "<li><a href=\"/home/pager?page=2&pageSize=10\">2</a></li>" +
                "<li><a href=\"/home/pager?page=3&pageSize=10\">3</a></li>" +
                "<li><a href=\"/home/pager?page=4&pageSize=10\">4</a></li>" +
                "<li class=\"active\"><a href=\"/home/pager?page=5&pageSize=10\">5</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li class=\"disabled\"><a>Next &#8250;</a></li>" +
                "<li class=\"disabled\"><a>&#187;</a></li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when navigating to middle page] generates expected output")]
        public void Process_OnMiddlePage_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.Page = 5;
            paginationTagHelper.PageSize = 5;
            paginationTagHelper.TotalItems = 50;
            paginationTagHelper.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={paginationTagHelper.Page}&pageSize={paginationTagHelper.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
               "<li><a href=\"/home/pager?page=1&pageSize=5\">&#171;</a></li>" +
               "<li><a href=\"/home/pager?page=4&pageSize=5\">&#8249; Previous</a></li>" +
               "<li><a href=\"/home/pager?page=2&pageSize=5\">&#46;&#46;</a></li>" +
               "<li><a href=\"/home/pager?page=3&pageSize=5\">3</a></li>" +
               "<li><a href=\"/home/pager?page=4&pageSize=5\">4</a></li>" +
               "<li class=\"active\"><a href=\"/home/pager?page=5&pageSize=5\">5</a></li>" +
               "<li><a href=\"/home/pager?page=6&pageSize=5\">6</a></li>" +
               "<li><a href=\"/home/pager?page=7&pageSize=5\">7</a></li>" +
               "<li><a href=\"/home/pager?page=8&pageSize=5\">&#46;&#46;</a></li>" +
               "<li><a href=\"/home/pager?page=6&pageSize=5\">Next &#8250;</a></li>" +
               "<li><a href=\"/home/pager?page=10&pageSize=5\">&#187;</a></li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when navigating past last page] generates expected output")]
        public void Process_NavigatePastLastPage_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.Page = 6;
            paginationTagHelper.PageSize = 10;
            paginationTagHelper.TotalItems = 50;
            paginationTagHelper.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={paginationTagHelper.Page}&pageSize={paginationTagHelper.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                "<li><a href=\"/home/pager?page=1&pageSize=10\">&#171;</a></li>" +
                "<li><a href=\"/home/pager?page=4&pageSize=10\">&#8249; Previous</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li><a href=\"/home/pager?page=1&pageSize=10\">1</a></li>" +
                "<li><a href=\"/home/pager?page=2&pageSize=10\">2</a></li>" +
                "<li><a href=\"/home/pager?page=3&pageSize=10\">3</a></li>" +
                "<li><a href=\"/home/pager?page=4&pageSize=10\">4</a></li>" +
                "<li class=\"active\"><a href=\"/home/pager?page=5&pageSize=10\">5</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li class=\"disabled\"><a>Next &#8250;</a></li>" +
                "<li class=\"disabled\"><a>&#187;</a></li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when no page results] generates expected output")]
        public void Process_NoPageResults_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.Page = 1;
            paginationTagHelper.PageSize = 10;
            paginationTagHelper.TotalItems = 0;
            paginationTagHelper.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={paginationTagHelper.Page}&pageSize={paginationTagHelper.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml = "";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [ajax request] generates expected output")]
        public void Process_AjaxRequest_GeneratesExpectedOutput()
        {
            // Arrange
            var ajaxOptionsTagHelper = new AjaxOptionsTagHelper();
            ajaxOptionsTagHelper.UpdateTargetId = "pagedContent";
            ajaxOptionsTagHelper.OnBegin = "showAjaxLoader()";
            ajaxOptionsTagHelper.OnComplete = "hideAjaxLoader()";

            var paginationTagHelper = new PaginationTagHelper();
            paginationTagHelper.Page = 1;
            paginationTagHelper.PageSize = 5;
            paginationTagHelper.TotalItems = 50;
            paginationTagHelper.Url = "/home/ajaxpager";
            paginationTagHelper.AjaxOptions = ajaxOptionsTagHelper;
            var query = QueryHelpers.ParseQuery($"page={paginationTagHelper.Page}&pageSize={paginationTagHelper.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            paginationTagHelper.ViewContext = viewContext;

            var tagHelperContext = new TagHelperContext(
                new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml = "<li class=\"disabled\"><a>&#171;</a></li>" +
                "<li class=\"disabled\"><a>&#8249; Previous</a></li>" +
                "<li class=\"disabled\"><a>&#46;&#46;</a></li>" +
                "<li class=\"active\"><a href=\"/home/ajaxpager?page=1&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">1</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=2&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">2</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=3&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">3</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=4&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">4</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=5&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">5</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=6&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">&#46;&#46;</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=2&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">Next &#8250;</a></li>" +
                "<li><a href=\"/home/ajaxpager?page=10&pageSize=5\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"showAjaxLoader()\" data-ajax-complete=\"hideAjaxLoader()\" data-ajax-update=\"#pagedContent\" data-ajax-mode=\"replace\">&#187;</a></li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }
    }
}
