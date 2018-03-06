using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PaginationTagHelper.AspNetCore.Tests
{
    public class PaginationTagHelperInfoTests
    {
        [Fact(DisplayName = "Process [show page items] generates expected output")]
        public void Process_ShowPageItems_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationInfoTagHelper = new PaginationInfoTagHelper();
            paginationInfoTagHelper.TextMode = TextMode.ShowPageItems;
            paginationInfoTagHelper.Page = 2;
            paginationInfoTagHelper.PageSize = 10;
            paginationInfoTagHelper.TotalItems = 500;


            var tagHelperContext = new TagHelperContext(
               new TagHelperAttributeList(),
               new Dictionary<object, object>(),
               Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination-info",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationInfoTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml = "<li>Showing 11 &ndash; 20 of 500 items</li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }

        [Fact(DisplayName = "Process [show page numbers] generates expected output")]
        public void Process_ShowPageNumbers_GeneratesExpectedOutput()
        {
            // Arrange
            var paginationInfoTagHelper = new PaginationInfoTagHelper();
            paginationInfoTagHelper.TextMode = TextMode.ShowPageNumbers;
            paginationInfoTagHelper.Page = 2;
            paginationInfoTagHelper.PageSize = 10;
            paginationInfoTagHelper.TotalItems = 500;


            var tagHelperContext = new TagHelperContext(
               new TagHelperAttributeList(),
               new Dictionary<object, object>(),
               Guid.NewGuid().ToString("N"));

            var tagHelperOutput = new TagHelperOutput("pagination-info",
                new TagHelperAttributeList(),
                (result, encoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetHtmlContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            // Act
            paginationInfoTagHelper.Process(tagHelperContext, tagHelperOutput);

            // Assert
            var expectedHtml = "<li>Page 2 of 50</li>";

            var actualHtml = tagHelperOutput.Content.GetContent();

            Assert.Equal(expectedHtml, actualHtml);
        }
    }
}
