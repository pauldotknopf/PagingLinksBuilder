PagingLinksBuilder.cs
==================

A single .cs file for you to add to your project. It quickly adds a very configurable paging control to your project. It renders out-of-the-box Twitter Bootstrap fiendly too!

![PagingLinksBuidler preview](https://raw.github.com/theonlylawislove/PagingLinksBuilder/master/preview.jpg)

# Getting Started

Add [the only file](https://raw.github.com/theonlylawislove/PagingLinksBuilder/master/PagingLinksBuilder.cs) you need to your project.

Add the following to an HtmlHelper extension method somewhere.

```c#
public static PagingLinksBuilder PagingLinksBuilder
	(this HtmlHelper helper, 
	int currentPage, 
	int totalPages, 
	Func<int, string> pageUrlBuilder)
{
    return new PagingLinksBuilder(currentPage, totalPages, pageUrlBuilder);
}
```

Then use the following in your views.

```c#
@Html.PagingLinksBuilder(1, 10, (page) => "/somewhere?p=" + page)
```

You are ready to rock-n-roll!

#Configuration/Defaults

The following represents a complete configuration of the defaults.

```c#
@(Html.PagingLinksBuilder(1, 10, (page) => "/somewhere?p=" + page)
    .LayoutTemplate(
        @<div class="pagination">
            <ul>@item</ul>
        </div>)
    .ItemTemplate(
        @<li class="@item.CssClass">@item.LinkRender</li>)
    .LinkTemplate(
        @<a href="@item.GetPageUrl()" class="@item.CssClass">@item.LinkText</a>)
    .MaxNumberOfTrailingLeadingPages(2)
    .AlwaysShowNavigation(true)
    .FirstCssClass("first")
    .PreviousCssClass("previous")
    .NextCssClass("next")
    .LastCssClass("last")
    .PageCssClass("page")
    .DisabledCssClass("disabled")
    .ActiveCssClass("active"))  
```

Enjoy!