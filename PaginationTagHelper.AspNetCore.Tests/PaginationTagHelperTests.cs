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
            var pth = new PaginationTagHelper();
            pth.TotalItems = 50;

            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery("");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                $"<ul class=\"pagination\">" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.FirstPageAriaLabel}\">{pth.FirstPageLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\">{pth.PreviousLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipBackAriaLabel.Replace("{0}", "1")}\">{pth.SkipBackPageLink}</a></li>" +
                    $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 1\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">1</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 2\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">2</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/pager?page=3&pageSize={pth.PageSize}\">3</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">4</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 5\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">5</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipForwardAriaLabel.Replace("{0}","5")}\">{pth.SkipForwardPageLink}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">{pth.NextLinkText}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.LastPageAriaLabel}\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">{pth.LastPageLinkText}</a></li>" +
                $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName="Process [when navigating to first page] generates expected output")]
        public void Process_OnFirstPage_GeneratesExpectedOutput()
        {
            // Arrange
            var pth = new PaginationTagHelper();
            pth.Page = 1;
            pth.PageSize = 10;
            pth.TotalItems = 50;
            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                $"<ul class=\"pagination\">" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.FirstPageAriaLabel}\">{pth.FirstPageLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\">{pth.PreviousLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipBackAriaLabel.Replace("{0}", "1")}\">{pth.SkipBackPageLink}</a></li>" +
                    $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 1\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">1</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 2\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">2</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/pager?page=3&pageSize={pth.PageSize}\">3</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">4</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 5\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">5</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipForwardAriaLabel.Replace("{0}", "5")}\">{pth.SkipForwardPageLink}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">{pth.NextLinkText}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.LastPageAriaLabel}\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">{pth.LastPageLinkText}</a></li>" +
                $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when navigating to last page] generates expected output")]
        public void Process_OnLastPage_GeneratesExpectedOutput()
        {
            // Arrange
            var pth = new PaginationTagHelper();
            pth.Page = 5;
            pth.PageSize = 10;
            pth.TotalItems = 50;
            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                $"<ul class=\"pagination\">" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.FirstPageAriaLabel}\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">{pth.FirstPageLinkText}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">{pth.PreviousLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipBackAriaLabel.Replace("{0}", "1")}\">{pth.SkipBackPageLink}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 1\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">1</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 2\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">2</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/pager?page=3&pageSize={pth.PageSize}\">3</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">4</a></li>" +
                    $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 5\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">5</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipForwardAriaLabel.Replace("{0}", "5")}\">{pth.SkipForwardPageLink}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\">{pth.NextLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.LastPageAriaLabel}\">{pth.LastPageLinkText}</a></li>" +
                $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when navigating to middle page] generates expected output")]
        public void Process_OnMiddlePage_GeneratesExpectedOutput()
        {
            // Arrange
            var pth = new PaginationTagHelper();
            pth.Page = 5;
            pth.PageSize = 5;
            pth.TotalItems = 50;
            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
               $"<ul class=\"pagination\">" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.FirstPageAriaLabel}\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">{pth.FirstPageLinkText}</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">{pth.PreviousLinkText}</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.SkipBackAriaLabel.Replace("{0}", "1")}\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">{pth.SkipBackPageLink}</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/pager?page=3&pageSize={pth.PageSize}\">3</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">4</a></li>" +
                   $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 5\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">5</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 6\" href=\"/home/pager?page=6&pageSize={pth.PageSize}\">6</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 7\" href=\"/home/pager?page=7&pageSize={pth.PageSize}\">7</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.SkipForwardAriaLabel.Replace("{0}", "10")}\" href=\"/home/pager?page=10&pageSize={pth.PageSize}\">{pth.SkipForwardPageLink}</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\" href=\"/home/pager?page=6&pageSize={pth.PageSize}\">{pth.NextLinkText}</a></li>" +
                   $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.LastPageAriaLabel}\" href=\"/home/pager?page=10&pageSize={pth.PageSize}\">{pth.LastPageLinkText}</a></li>" +
               $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when navigating past last page] generates expected output")]
        public void Process_NavigatePastLastPage_GeneratesExpectedOutput()
        {
            // Arrange
            var pth = new PaginationTagHelper();
            pth.Page = 6;
            pth.PageSize = 10;
            pth.TotalItems = 50;
            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                $"<ul class=\"pagination\">" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.FirstPageAriaLabel}\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">{pth.FirstPageLinkText}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">{pth.PreviousLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipBackAriaLabel.Replace("{0}", "1")}\">{pth.SkipBackPageLink}</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 1\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">1</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 2\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">2</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/pager?page=3&pageSize={pth.PageSize}\">3</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">4</a></li>" +
                    $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 5\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">5</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipForwardAriaLabel.Replace("{0}", "5")}\">{pth.SkipForwardPageLink}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\">{pth.NextLinkText}</a></li>" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.LastPageAriaLabel}\">{pth.LastPageLinkText}</a></li>" +
                $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when no page results] generates expected output")]
        public void Process_NoPageResults_GeneratesExpectedOutput()
        {
            // Arrange
            var pth = new PaginationTagHelper();
            pth.Page = 1;
            pth.PageSize = 10;
            pth.TotalItems = 0;
            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

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

            var pth = new PaginationTagHelper();
            pth.Page = 1;
            pth.PageSize = 5;
            pth.TotalItems = 50;
            pth.Url = "/home/ajaxpager";
            pth.AjaxOptions = ajaxOptionsTagHelper;
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
            $"<ul class=\"pagination\">" +
                $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.FirstPageAriaLabel}\">&#171;</a></li>" +
                $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\">&#8249; Previous</a></li>" +
                $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.SkipBackAriaLabel.Replace("{0}", "1")}\">&#46;&#46;</a></li>" +
                $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 1\" href=\"/home/ajaxpager?page=1&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">1</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 2\" href=\"/home/ajaxpager?page=2&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">2</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/ajaxpager?page=3&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">3</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/ajaxpager?page=4&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">4</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 5\" href=\"/home/ajaxpager?page=5&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">5</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.SkipForwardAriaLabel.Replace("{0}", "6")}\" href=\"/home/ajaxpager?page=6&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">&#46;&#46;</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\" href=\"/home/ajaxpager?page=2&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">Next &#8250;</a></li>" +
                $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.LastPageAriaLabel}\" href=\"/home/ajaxpager?page=10&pageSize={pth.PageSize}\" data-ajax=\"true\" data-ajax-method=\"Post\" data-ajax-begin=\"{pth.AjaxOptions.OnBegin}\" data-ajax-complete=\"{pth.AjaxOptions.OnComplete}\" data-ajax-update=\"#{pth.AjaxOptions.UpdateTargetId}\" data-ajax-mode=\"replace\">&#187;</a></li>" +
            $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [when first/last and skip navigation links disabled] generates expected output")]
        public void Process_OnDisableFirstLastAndSkipNavigationLinks_GeneratesExpectedOutput()
        {
            // Arrange
            var pth = new PaginationTagHelper();
            pth.Page = 1;
            pth.PageSize = 10;
            pth.TotalItems = 50;
            pth.FirstAndLastPageNavigation = NavigationFeature.Disabled;
            pth.SkipForwardAndBackNavigation = NavigationFeature.Disabled;
            pth.Url = "/home/pager";
            var query = QueryHelpers.ParseQuery($"page={pth.Page}&pageSize={pth.PageSize}");

            request.Setup(x => x.Query).Returns(new QueryCollection(query));
            context.Setup(x => x.HttpContext.Request).Returns(request.Object);
            viewContext.HttpContext = context.Object.HttpContext;

            pth.ViewContext = viewContext;

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
            pth.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml =
                $"<ul class=\"pagination\">" +
                    $"<li class=\"page-item disabled\" tabindex=\"-1\"><a class=\"page-link\" aria-label=\"{pth.PreviousPageAriaLabel}\">{pth.PreviousLinkText}</a></li>" +
                    $"<li class=\"page-item active\"><a class=\"page-link\" aria-current=\"true\" aria-label=\"{pth.CurrentPageAriaLabel} 1\" href=\"/home/pager?page=1&pageSize={pth.PageSize}\">1</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 2\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">2</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 3\" href=\"/home/pager?page=3&pageSize={pth.PageSize}\">3</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 4\" href=\"/home/pager?page=4&pageSize={pth.PageSize}\">4</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.GotoPageAriaLabel} 5\" href=\"/home/pager?page=5&pageSize={pth.PageSize}\">5</a></li>" +
                    $"<li class=\"page-item\"><a class=\"page-link\" aria-label=\"{pth.NextPageAriaLabel}\" href=\"/home/pager?page=2&pageSize={pth.PageSize}\">{pth.NextLinkText}</a></li>" +
                $"</ul>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }
    }
}
