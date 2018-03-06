using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace PaginationTagHelper.AspNetCore
{
    public enum TextMode
    {
        ShowPageNumbers,
        ShowPageItems
    }

    [HtmlTargetElement("pager-info", Attributes = "page, page-size, total-items")]
    public class PaginationInfoTagHelper : TagHelper
    {
        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public TextMode TextMode { get; set; } = TextMode.ShowPageItems;

        public string CssClass { get; set; } = "pagination";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "<ul>";
            output.Attributes.Add("class", $"{CssClass}");
            output.TagMode = TagMode.StartTagAndEndTag;

            if (TextMode == TextMode.ShowPageItems)
                RenderPageItemText(output);
            else
                RenderPageNumberText(output);

            output.PostContent.SetHtmlContent("</ul>");
            output.TagName = "ul";
        }

        void RenderPageNumberText(TagHelperOutput output)
        {
            // Calculate the total number of pages.
            int totalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            int firstItem = (Page * PageSize) - (PageSize - 1);
            int lastItem = firstItem + PageSize - 1;
            if (lastItem > TotalItems)
                lastItem = TotalItems;

            output.Content.SetHtmlContent($@"<li>Page {Page} of {totalPages}</li>");
        }

        void RenderPageItemText(TagHelperOutput output)
        {
            int firstItem = (Page * PageSize) - (PageSize - 1);
            int lastItem = firstItem + PageSize - 1;
            if (lastItem > TotalItems)
                lastItem = TotalItems;

            output.Content.SetHtmlContent($@"<li>Showing {firstItem} &ndash; {lastItem} of {TotalItems} items</li>");
        }
    }
}
