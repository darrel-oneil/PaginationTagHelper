# PaginationTagHelper.AspNetCore

An easy to use paging tag helper with ajax support and Bootstrap 4.0 generated markup. 

![Alt text](https://user-images.githubusercontent.com/2548239/37115229-74da5bc0-2242-11e8-8684-331891b2d8c2.png "Bootstrap 3 Paging Markup")

Example usage
--------
~~~
<pager link-url="/home/pager"
       page="Model.CurrentPage"
       page-size="Model.PageSize"
       total-items="Model.TotalItems">
</pager>
~~~

Ajax usage
----------
To use the paging tag helper with ajax you just need to nest an `<ajax-options></ajax-options>` tag helper inside the pager tag helper and add the required data-ajax-* attributes. 

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

| ajax-options attribute| Generated HTML5 data-* element
| --------------------  | ------------------------
| confirm	 		          | data-ajax-confirm |
| method		 		        | data-ajax-method |
| mode			 		        | data-ajax-mode |
| loading-element-id    | data-ajax-loading-element-id |
| loading-duration	 	  | data-ajax-loading-duration |
| on-begin				      | data-ajax-begin |
| on-complete		 	      | data-ajax-complete |
| on-failure			      | data-ajax-failure |
| on-success		 	      | data-ajax-success |
| update-target-id	 	  | data-ajax-update |
| link-url				      | data-ajax-url |
| allow-cache			      | data-ajax-cache |


# Sample Web application
Please refer to the sample *PaginationTagHelper.AspNetCore.Web* application in this repo which helps demonstrate how the pagination tag helper can be used. This web project currently uses Bootstrap 3.3.
