# PaginationTagHelper.AspNetCore

An easy to use paging tag helper with ajax support and Bootstrap 4.0 generated markup.

Please refer to the sample *PaginationTagHelper.AspNetCore.Web* application in this repo for an example on how you can use this tag helper in your own projects.

The sample web project currently uses Bootstrap 3.3.

Paging tag helper attributes (required)
---
| Attribute | Description | Example
| :--- | :--- | :---
| link-url| Used to construct the page links | home/index
| page | The current page number | 1
| page-size | The number of items to display per page | 10
| total-items | The total number of items to page| 500

**Sample Markup**

To be added to the view.

```
<pager link-url="/home/index"
       page="Model.CurrentPage"
       page-size="Model.PageSize"
       total-items="Model.TotalItems">
</pager>
```

Optional attributes
---
The following attributes are automatically rendered with a default value and can be overridden by adding them to the `<pager>` tag.

| Attribute | Description | Default
| :--- | :--- | :---
| pages-to-display | The number of visible page links to render	| 5
| css-class | Adds a *class* attribute to the bootstrap pagination `<ul>` tag | pagination
| first-page-text | The text displayed on the first page link | &#171;
| previous-page-text | The text displayed on the previous page link | &#8249; Previous
| skip-back-text | The text displayed on the skip back page link | &#46;&#46;
| skip-forward-text | The text displayed on the skip forward page link | &#46;&#46;
| next-page-text | The text displayed on the next page link | Next &#8250;
| next-page-aria-label | Adds an *aria-label* attribute to the next page link | go to next page
| last-page-text | The text displayed on the last page link | &#187;
| first-last-navigation | Show/hide the [&#171; first] and [last &#187;] page links | Enabled
| skip-forward-back-navigation | Show/hide the [..] skip forward/back page links | Enabled


**Accessibility attributes**

The following ARIA attributes are automatically rendered with a default value and can be overridden by adding them to the `<pager>` tag.

| Attribute | Description | Default
| :---| :---| :---
| aria-label | Adds an *aria-label* attribute to the pagination `<nav>` tag | pagination
| previous-page-aria-label | Adds an *aria-label* attribute to previous page link | go to previous page
| skip-back-aria-label |  Adds an *aria-label* attribute to skip back page link | skip back to page {0}
| skip-forward-aria-label | Adds an *aria-label* attribute to skip forward page link | skip forward to page {0}
| next-page-aria-label | Adds an *aria-label* attribute to next page link  | go to next page
| last-page-aria-label | Adds an *aria-label* attribute to last page link | go to last page
| current-page-aria-label | Adds an *aria-label* and *aria-current* attribute to skip back page link | page
| goto-page-aria-label |  Adds an *aria-label* attribute to numeric page links | go to page

**Note:** *{0} will be replaced with the page number if present.*

**Ajax attributes**

To use ajax nest an `<ajax-options>` tag helper inside the `<pager>` tag helper and add the required data-ajax-* attributes.

| ajax-options attribute | Generated HTML5 data-* element
| :---  | :---
| confirm | data-ajax-confirm
| mode | data-ajax-mode
| loading-element-id | data-ajax-loading-element-id
| loading-duration | data-ajax-loading-duration
| on-begin | data-ajax-begin
| on-complete | data-ajax-complete
| on-failure | data-ajax-failure
| on-success | data-ajax-success
| update-target-id | data-ajax-update
| link-url | data-ajax-url
| allow-cache | data-ajax-cache

**Sample markup**

~~~
<pager link-url="/home/ajaxpager"
       page="Model.CurrentPage"
       page-size="Model.PageSize"
       total-items="Model.TotalItemCount">
       <ajax-options update-target-id="pagedContent"
           on-begin="showAjaxLoader()"
           on-complete="hideAjaxLoader()">
       </ajax-options>
</pager>
~~~
