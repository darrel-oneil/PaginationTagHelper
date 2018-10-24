using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;

namespace PaginationTagHelper.AspNetCore
{
    public enum NavigationFeature
    {
        Disabled,
        Enabled
    }

    [HtmlTargetElement("pager", Attributes = "link-url, page, total-items")]
    public class PaginationTagHelper : TagHelper
    {
        // Adds class name(s) to the bootstrap pagination unordered list e.g. "<ul class="pagination pagination-lg">".
        [HtmlAttributeName("css-class")]
        public string CssClass { get; set; } = "pagination";

        // Accessibility: Adds 'aria-label' property to the bootstrap pagination <nav> tag (e.g. <nav aria-label="Pagination">).
        [HtmlAttributeName("aria-label")]
        public string AriaLabel { get; set; } = "Pagination";

        // The text displayed on the "«" go to first page link. 
        [HtmlAttributeName("first-page-text")]
        public string FirstPageLinkText { get; set; } = "&#171;";

        // Accessibility: Adds 'aria-label' to the "«" go to first page link. (e.g. aria-label="go to first page").
        [HtmlAttributeName("first-page-aria-label")]
        public string FirstPageAriaLabel { get; set; } = "go to first page";

        // The text displayed on the "previous page" navigation link. 
        [HtmlAttributeName("previous-page-text")]
        public string PreviousLinkText { get; set; } = "&#8249; Previous";

        // Accessibility: Adds 'aria-label' to the previous page link. (e.g. aria-label="go to previous page").
        [HtmlAttributeName("previous-page-aria-label")]
        public string PreviousPageAriaLabel { get; set; } = "go to previous page";

        // The text displayed on the ".." skip back to page link.
        [HtmlAttributeName("skip-back-text")]
        public string SkipBackPageLink { get; set; } = "&#46;&#46;";

        // Accessibility: Adds 'aria-label' to the "«" first page link (e.g. aria-label="skip back to page 10").
        [HtmlAttributeName("skip-back-aria-label")]
        public string SkipBackAriaLabel { get; set; } = "skip back to page {0}";

        // The text displayed on the ".." skip forward page link.
        [HtmlAttributeName("skip-forward-text")]
        public string SkipForwardPageLink { get; set; } = "&#46;&#46;";

        // Accessibility: Adds 'aria-label' to the ".." skip forward page link (e.g. aria-label="skip forward to page 10").
        [HtmlAttributeName("skip-forward-aria-label")]
        public string SkipForwardAriaLabel { get; set; } = "skip forward to page {0}";

        // The text displayed on the next page link.
        [HtmlAttributeName("next-page-text")]
        public string NextLinkText { get; set; } = "Next &#8250;";

        // Accessibility: Adds 'aria-label' to the next page link (e.g. aria-label="go to next page").
        [HtmlAttributeName("next-page-aria-label")]
        public string NextPageAriaLabel { get; set; } = "go to next page";

        // The text displayed on the "»" go to last page link. 
        [HtmlAttributeName("last-page-text")]
        public string LastPageLinkText { get; set; } = "&#187;";

        // Accessibility: Adds 'aria-label' to the "»" go to last page link. (e.g. aria-label="go to last page").
        [HtmlAttributeName("last-page-aria-label")]
        public string LastPageAriaLabel { get; set; } = "go to last page";

        // Accessibility: Adds 'aria-current' and 'aria-label' to the active page link (e.g. aria-current="true" aria-label="page 1").
        [HtmlAttributeName("current-page-aria-label")]
        public string CurrentPageAriaLabel { get; set; } = "page";

        // Accessibility: Adds 'aria-label' to the numbered page links. (e.g. aria-label="go to page 3").
        [HtmlAttributeName("goto-page-aria-label")]
        public string GotoPageAriaLabel { get; set; } = "go to page";

        // The calling pages url (e.g. "/home/index")
        [HtmlAttributeName("link-url")]
        public string Url { get; set; }

        // The currently active page number.
        public int Page { get; set; } = 1;

        // The number of items to display per page.
        public int PageSize { get; set; } = 10;

        // The total number of items to page.
        public int TotalItems { get; set; }

        // The number of visible page links to display in the pagination control.
        public int PagesToDisplay { get; set; } = 5;

        // Controls the rendering of the [« go to first page] and [go to last page »] page links.
        [HtmlAttributeName("first-last-navigation")]
        public NavigationFeature FirstAndLastPageNavigation { get; set; } = NavigationFeature.Enabled;

        // Controls the rendering of the [..] skip forward and back page links. You may wish to disable on smaller data sets.
        [HtmlAttributeName("skip-forward-back-navigation")]
        public NavigationFeature SkipForwardAndBackNavigation { get; set; } = NavigationFeature.Enabled;

        // Child tag helper used to add the ajax options.
        public AjaxOptionsTagHelper AjaxOptions { get; set; }

        // Required to get access to the current view context.
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        // --------------------------------------------------
        // Private properties
        // --------------------------------------------------

        int FirstVisiblePage { get; set; }

        int LastVisiblePage { get; set; }

        int SkipPagesForward { get; set; }

        int SkipPagesBack { get; set; }

        int TotalPages { get; set; }

        bool SupressOutput { get; set; } = false;

        RouteValueDictionary RouteValues { get; set; }

        string AjaxHtmlAttributes {get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            context.Items.Add(typeof(PaginationTagHelper), this);
            output.GetChildContentAsync();

            output.TagName = "nav";
            output.Attributes.Add("role", "navigation");
            output.Attributes.Add("aria-label", AriaLabel);
            output.Attributes.Add("class", "pagination-tag-helper");

            output.Content.AppendHtml($@"<ul class=""{CssClass}"">");

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
            // [«] [Previous] [..] [1] [2] [3] [4] [5] [..] [Next] [»]
            // --------------------------------------------
            CalculatePageNumbers();

            if (SupressOutput)
            {
                output.SuppressOutput();
            }
            else
            {
                if (FirstAndLastPageNavigation == NavigationFeature.Enabled)
                    BuildFirstPageLink(output);

                BuildPreviousPageLink(output);

                if (SkipForwardAndBackNavigation == NavigationFeature.Enabled)
                    BuildSkipPagesBackLink(output);

                BuildPageNumberLinks(output);

                if (SkipForwardAndBackNavigation == NavigationFeature.Enabled)
                    BuildSkipPagesForwardLink(output);

                BuildNextPageLink(output);

                if (FirstAndLastPageNavigation == NavigationFeature.Enabled)
                    BuildLastPageLink(output);

                output.Content.AppendHtml("</ul>");
            }
        }

        /// <summary>
        /// Builds the "back to first page" anchor tag [«]
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildFirstPageLink(TagHelperOutput output)
        {
            if (TotalPages > 1 && Page > 1)
            {
                RouteValues["page"] = "1";
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{FirstPageAriaLabel}"" href=""{url}""{AjaxHtmlAttributes}>{FirstPageLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""page-item disabled"" tabindex=""-1""><a class=""page-link"" aria-label=""{FirstPageAriaLabel}"">{FirstPageLinkText}</a></li>");
            }
        }

        /// <summary>
        /// Builds the "previous page" anchor tag [« Previous]
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildPreviousPageLink(TagHelperOutput output)
        {
            if (Page > 1)
            {
                RouteValues["page"] = (Page - 1).ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{PreviousPageAriaLabel}"" href=""{url}""{AjaxHtmlAttributes}>{PreviousLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""page-item disabled"" tabindex=""-1""><a class=""page-link"" aria-label=""{PreviousPageAriaLabel}"">{PreviousLinkText}</a></li>");
            }
        }

        /// <summary>
        ///  Builds the ".." anchor tag for navigating back through the pages.
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildSkipPagesBackLink(TagHelperOutput output)
        {
            string ariaLabel = SkipBackAriaLabel;
            if (SkipBackAriaLabel.Contains("{0}"))
                ariaLabel = String.Format(SkipBackAriaLabel, SkipPagesBack);

            if (Page > 1 && FirstVisiblePage > 1)
            {
                RouteValues["page"] = SkipPagesBack.ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{ariaLabel}"" href=""{url}""{AjaxHtmlAttributes}>{SkipBackPageLink}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""page-item disabled"" tabindex=""-1""><a class=""page-link"" aria-label=""{ariaLabel}"">{SkipBackPageLink}</a></li>");
            }
        }

        /// <summary>
        ///  Builds the numbered anchor tags for navigating to a specific page.
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildPageNumberLinks(TagHelperOutput output)
        {
            // Create all visible page links
            for (int pageNumber = FirstVisiblePage; pageNumber <= LastVisiblePage; pageNumber++)
            {
                RouteValues["page"] = pageNumber.ToString();
                string url = ConstructPageLinkUrl(RouteValues);

                if (pageNumber == Page)
                {
                    output.Content.AppendHtml($@"<li class=""page-item active""><a class=""page-link"" aria-current=""true"" aria-label=""{CurrentPageAriaLabel} {pageNumber}"" href=""{url}""{AjaxHtmlAttributes}>{pageNumber}</a></li>");
                }
                else
                {
                    output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{GotoPageAriaLabel} {pageNumber}"" href=""{url}""{AjaxHtmlAttributes}>{pageNumber}</a></li>");
                }
            }
        }

        /// <summary>
        ///  Builds the ".." anchor tag for skipping forward through the pages.
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildSkipPagesForwardLink(TagHelperOutput output)
        {
            string ariaLabel = SkipForwardAriaLabel;
            if (SkipForwardAriaLabel.Contains("{0}"))
                ariaLabel = String.Format(SkipForwardAriaLabel, SkipPagesForward);

            if (TotalPages > 1 && LastVisiblePage < TotalPages)
            {
                RouteValues["page"] = SkipPagesForward.ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{ariaLabel}"" href=""{url}""{AjaxHtmlAttributes}>{SkipForwardPageLink}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""page-item disabled"" tabindex=""-1""><a class=""page-link"" aria-label=""{ariaLabel}"">{SkipForwardPageLink}</a></li>");
            }
        }

        /// <summary>
        /// Builds the "next page" anchor tag [»]
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildNextPageLink(TagHelperOutput output)
        {
            if (Page < TotalPages)
            {
                RouteValues["page"] = (Page + 1).ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{NextPageAriaLabel}"" href=""{url}""{AjaxHtmlAttributes}>{NextLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""page-item disabled"" tabindex=""-1""><a class=""page-link"" aria-label=""{NextPageAriaLabel}"">{NextLinkText}</a></li>");
            }
        }

        /// <summary>
        /// Builds the "last page" anchor tag [»»]
        /// </summary>
        /// <param name="output">Appends html to the parent TagHelperOutput object</param>
        void BuildLastPageLink(TagHelperOutput output)
        {
            if (TotalPages > 1 && Page < TotalPages)
            {
                RouteValues["page"] = TotalPages.ToString();
                string url = ConstructPageLinkUrl(RouteValues);
                output.Content.AppendHtml($@"<li class=""page-item""><a class=""page-link"" aria-label=""{LastPageAriaLabel}"" href=""{url}""{AjaxHtmlAttributes}>{LastPageLinkText}</a></li>");
            }
            else
            {
                output.Content.AppendHtml($@"<li class=""page-item disabled"" tabindex=""-1""><a class=""page-link"" aria-label=""{LastPageAriaLabel}"">{LastPageLinkText}</a></li>");
            }
        }

        /// <summary>
        /// Constructs the paging link urls (e.g. home/index?page=1&amp;pageSize=10).
        /// </summary>
        /// <param name="parameters">Contains the paging links querystring parameters (e.g. page=1, pageSize=10).</param>
        /// <returns>Paging link url string</returns>
        string ConstructPageLinkUrl(RouteValueDictionary parameters)
        {
            var url = String.Format("{0}?{1}", Url,
                      string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value))));

            return url;
        }

        /// <summary>
        /// Sets the paging link variables on each request.
        /// </summary>
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

            // Calculate the skip pages back link.
            SkipPagesBack = Page - PagesToDisplay;
            if (SkipPagesBack < 1)
                SkipPagesBack = 1;

            // Calculate the more pages forward link.
            SkipPagesForward = Page + PagesToDisplay;
            if (SkipPagesForward > TotalPages)
                SkipPagesForward = TotalPages;
        }

        /// <summary>
        /// Parses all querystring key/value pairs on each request.
        /// </summary>
        /// <returns>RouteValueDictionary object</returns>
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
