using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PaginationTagHelper.AspNetCore
{
    //  Enumerates the AJAX script insertion modes.
    public enum InsertionMode
    {
        // Replace the element.
        Replace = 0,

        // Insert before the element.
        InsertBefore = 1,

        // Insert after the element.
        InsertAfter = 2,

        // Replace the entire element.
        ReplaceWith = 3
    }

    [HtmlTargetElement("ajax-options")]
    public class AjaxOptionsTagHelper : TagHelper
    {
        private static readonly Regex _idRegex = new Regex(@"[.:[\]]");

        // Gets or sets the message to display in a confirmation window before a request is submitted.
        public string Confirm { get; set; }

        // Gets or sets the HTTP request method ("Get" or "Post").
        public string HttpMethod { get; set; } = "Post";

        // Gets or sets the mode that specifies how to insert the response into the target DOM element.
        public InsertionMode InsertionMode { get; set; } = InsertionMode.Replace;

        // Gets or sets a value, in milliseconds, that controls the duration of the animation
        // when showing or hiding the loading element.
        public int LoadingElementDuration { get; set; }

        // Gets or sets the id attribute of an HTML element that is displayed while the Ajax function is loading.
        public string LoadingElementId { get; set; }

        // Gets or sets the name of the JavaScript function to call immediately before the page is updated.
        public string OnBegin { get; set; }

        // Gets or sets the JavaScript function to call when response data has been instantiated but before the page is updated.
        public string OnComplete { get; set; }

        // Gets or sets the JavaScript function to call if the page update fails.
        public string OnFailure { get; set; }

        // Gets or sets the JavaScript function to call after the page is successfully updated.
        public string OnSuccess { get; set; }

        // Gets or sets the ID of the DOM element to update by using the response from the server.
        public string UpdateTargetId { get; set; }

        // Gets or sets the URL to make the request to.
        public string Url { get; set; }

        public bool AllowCache { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            ((PaginationTagHelper)context.Items[typeof(PaginationTagHelper)]).AjaxOptions = this;
            output.SuppressOutput();
        }

        // Returns the Ajax options as a collection of HTML attributes to support unobtrusive JavaScript.
        public IDictionary<string, object> ToUnobtrusiveHtmlAttributes()
        {
            var result = new Dictionary<string, object>
            {
                { "data-ajax", "true" },
            };

            AddToDictionaryIfSpecified(result, "data-ajax-url", Url);
            AddToDictionaryIfSpecified(result, "data-ajax-method", HttpMethod);
            AddToDictionaryIfSpecified(result, "data-ajax-confirm", Confirm);

            AddToDictionaryIfSpecified(result, "data-ajax-begin", OnBegin);
            AddToDictionaryIfSpecified(result, "data-ajax-complete", OnComplete);
            AddToDictionaryIfSpecified(result, "data-ajax-failure", OnFailure);
            AddToDictionaryIfSpecified(result, "data-ajax-success", OnSuccess);

            if (AllowCache)
            {
                // On the client, the absence of the data-ajax-cache attribute is equivalent to setting it to false.
                // Consequently we'll only set it if the user wants to opt into caching. 
                AddToDictionaryIfSpecified(result, "data-ajax-cache", "true");
            }

            if (!String.IsNullOrWhiteSpace(LoadingElementId))
            {
                result.Add("data-ajax-loading", EscapeIdSelector(LoadingElementId));

                if (LoadingElementDuration > 0)
                {
                    result.Add("data-ajax-loading-duration", LoadingElementDuration);
                }
            }

            if (!String.IsNullOrWhiteSpace(UpdateTargetId))
            {
                result.Add("data-ajax-update", EscapeIdSelector(UpdateTargetId));
                result.Add("data-ajax-mode", InsertionModeUnobtrusive);
            }

            return result;
        }

        internal string InsertionModeUnobtrusive
        {
            get
            {
                switch (InsertionMode)
                {
                    case InsertionMode.Replace:
                        return "replace";
                    case InsertionMode.InsertBefore:
                        return "before";
                    case InsertionMode.InsertAfter:
                        return "after";
                    case InsertionMode.ReplaceWith:
                        return "replace-with";
                    default:
                        return ((int)InsertionMode).ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        // Helpers
        private static void AddToDictionaryIfSpecified(IDictionary<string, object> dictionary, string name, string value)
        {
            if (!String.IsNullOrWhiteSpace(value))
            {
                dictionary.Add(name, value);
            }
        }

        private static string EscapeIdSelector(string selector)
        {
            // The string returned by this function is used as a value for jQuery's selector. The characters dot, colon and 
            // square brackets are valid id characters but need to be properly escaped since they have special meaning. For
            // e.g., for the id a.b, $('#a.b') would cause ".b" to treated as a class selector. The correct way to specify
            // this selector would be to escape the dot to get $('#a\.b').
            // See http://learn.jquery.com/using-jquery-core/faq/how-do-i-select-an-element-by-an-id-that-has-characters-used-in-css-notation/
            return '#' + _idRegex.Replace(selector, @"\$&");
        }
    }
}
