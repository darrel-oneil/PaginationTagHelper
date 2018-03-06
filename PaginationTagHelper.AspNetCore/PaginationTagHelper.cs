using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace PaginationTagHelper.AspNetCore
{
    [HtmlTargetElement("pager", Attributes = "link-url, page, total-items")]
    public class PaginationTagHelper : TagHelper
    {
        const string FirstPageLinkText = "&#171;";

        const string PreviousLinkText = "&#8249; Previous";

        const string SkipBackPageLink = "&#46;&#46;";

        const string SkipForwardPageLink = "&#46;&#46;";

        const string NextLinkText = "Next &#8250;";

        const string LastPageLinkText = "&#187;";

        [HtmlAttributeName("link-url")]
        public string Url { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalItems { get; set; }

        public int PagesToDisplay { get; set; } = 5;

        public AjaxOptionsTagHelper AjaxOptions { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        // --------------------------------------------------
        // Private properties
        // --------------------------------------------------

        int FirstVisiblePage { get; set; }

        int LastVisiblePage { get; set; }

        int MorePagesForward { get; set; }

        int MorePagesBack { get; set; }

        int TotalPages { get; set; }

        bool SupressOutput { get; set; } = false;

        RouteValueDictionary RouteValues { get; set; }

        string AjaxHtmlAttributes {get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            context.Items.Add(typeof(PaginationTagHelper), this);
            output.GetChildContentAsync();

            output.TagName = "ul";
            output.Attributes.Add("class", "pagination");
            output.TagMode = TagMode.StartTagAndEndTag;

            if (AjaxOptions != null)
            {
                var htmlAttributes = AjaxOptions.ToUnobtrusiveHtmlAttributes();
                AjaxHtmlAttributes = String.Join(" ", htmlAttributes.Select(kvp => kvp.Key + "=" + "\"" + kvp.Value + "\""));
                AjaxHtmlAttributes = " " + AjaxHtmlAttributes;
            }
            else
                AjaxHtmlAttributes = String.Empty;

            // Extracts the route values from the querystring.
            RouteValues = GetPagingRouteValues();

            // Create the paging links
            // --------------------------------------------
            // [«] [Previous] [..] [6] [7] [8] [9] [..] [Next] [»]
            // --------------------------------------------
            CalculatePageNumbers();

            if (SupressOutput)
            {
                output.SuppressOutput();
            }
            else
            { 
                BuildFirstPageLink(output);
                BuildPreviousPageLink(output);
                BuildMorePagesBackLink(output);
                BuildPageNumberLinks(output);
                BuildMorePagesForwardLink(output);
                BuildNextPageLink(output);
                BuildLastPageLink(output);
            }
           
            output.PostContent.SetHtmlContent("</ul>");
            output.TagName = "ul";
        }

        /// <summary>
        /// Builds the "back to first page" anchor tag [«]
        /// </summary>
        /// <param name="output"></param>
        void BuildFirstPageLink(TagHelperOutput output)
        {
            if (TotalPages > 1 && Page > 1)
            {
                RouteValues["page"] = "1";
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{FirstPageLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""disabled""><a>{FirstPageLinkText}</a></li>");
            }
        }

        /// <summary>
        /// Builds the "previous page" anchor tag [« Previous]
        /// </summary>
        /// <param name="output"></param>
        void BuildPreviousPageLink(TagHelperOutput output)
        {
            if (Page > 1)
            {
                RouteValues["page"] = (Page - 1).ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{PreviousLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""disabled""><a>{PreviousLinkText}</a></li>");
            }
        }

        /// <summary>
        ///  Builds the ".." anchor tag for navigating back through the pages.
        /// </summary>
        /// <param name="output"></param>
        void BuildMorePagesBackLink(TagHelperOutput output)
        {
            if (Page > 1 && FirstVisiblePage > 1)
            {
                RouteValues["page"] = MorePagesBack.ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{SkipBackPageLink}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""disabled""><a>{SkipBackPageLink}</a></li>");
            }
        }

        /// <summary>
        ///  Builds the numbered anchor tags for navigating to a specific page.
        /// </summary>
        /// <param name="output"></param>
        void BuildPageNumberLinks(TagHelperOutput output)
        {
            // Create all visible page links
            for (int pageNumber = FirstVisiblePage; pageNumber <= LastVisiblePage; pageNumber++)
            {
                RouteValues["page"] = pageNumber.ToString();
                string url = ConstructPageLinkUrl(RouteValues);

                if (pageNumber == Page)
                {
                    output.Content.AppendHtml($@"<li class=""active""><a href=""{url}""{AjaxHtmlAttributes}>{pageNumber}</a></li>");
                }
                else
                {
                    output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{pageNumber}</a></li>");
                }
            }
        }

        /// <summary>
        ///  Builds the ".." anchor tag for navigating forward through the pages.
        /// </summary>
        /// <param name="output"></param>
        void BuildMorePagesForwardLink(TagHelperOutput output)
        {
            if (TotalPages > 1 && LastVisiblePage < TotalPages)
            {
                RouteValues["page"] = MorePagesForward.ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{SkipForwardPageLink}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""disabled""><a>{SkipForwardPageLink}</a></li>");
            }
         
        }

        /// <summary>
        /// Builds the "next page" anchor tag [»]
        /// </summary>
        /// <param name="output"></param>
        void BuildNextPageLink(TagHelperOutput output)
        {
            if (Page < TotalPages)
            {
                RouteValues["page"] = (Page + 1).ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{NextLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""disabled""><a>{NextLinkText}</a></li>");
            }
        }

        /// <summary>
        /// Builds the "next page" anchor tag [»»]
        /// </summary>
        /// <param name="output"></param>
        void BuildLastPageLink(TagHelperOutput output)
        {
            if (TotalPages > 1 && Page < TotalPages)
            {
                RouteValues["page"] = TotalPages.ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li><a href=""{url}""{AjaxHtmlAttributes}>{LastPageLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""disabled""><a>{LastPageLinkText}</a></li>");
            }
        }

        string ConstructPageLinkUrl(RouteValueDictionary parameters)
        {
            var url = String.Format("{0}?{1}", Url,
                      string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))));

            return url;
        }

        void CalculatePageNumbers()
        {
            // Calculate the total number of pages.
            TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);

            // Supress the output if there are no pages to display.
            if (TotalPages == 0)
                SupressOutput = true;

            // Error handling.
            if (Page > TotalPages)
                Page = TotalPages;

            // Calculate the first visible page link.
            FirstVisiblePage = Page - (PagesToDisplay / 2);
            if (FirstVisiblePage + PagesToDisplay > TotalPages)
                FirstVisiblePage = TotalPages + 1 - PagesToDisplay;
            if (FirstVisiblePage < 1)
                FirstVisiblePage = 1;

            // Calculate the last visible page link.
            LastVisiblePage = FirstVisiblePage + PagesToDisplay - 1;
            if (LastVisiblePage > TotalPages)
                LastVisiblePage = TotalPages;

            // Calculate the more paged back link.
            MorePagesBack = FirstVisiblePage - 1;
            if (MorePagesBack < 1)
                MorePagesBack = 1;

            // Calculate the more pages forward link.
            MorePagesForward = FirstVisiblePage + PagesToDisplay;
            if (MorePagesForward > TotalPages)
                MorePagesForward = TotalPages;
        }

        RouteValueDictionary GetPagingRouteValues()
        {
            // e.g. /Home/PagingResults?page=2&pageSize=20
            var routeValues = new RouteValueDictionary();

            foreach (var query in ViewContext.HttpContext.Request.Query)
            {
                // Prevent duplicates and remove ajax request.
                if (query.Key != null && !routeValues.ContainsKey(query.Key) && query.Key.ToLower() != "x-requested-with" && query.Key != "_")
                {
                    routeValues.Add(query.Key, query.Value);
                }
            }

            // handle initial request
            if (!routeValues.ContainsKey("page"))
                routeValues.Add("page", Page);

            if (!routeValues.ContainsKey("pageSize"))
                routeValues.Add("pageSize", PageSize);

            return routeValues;
        }
    }
}
