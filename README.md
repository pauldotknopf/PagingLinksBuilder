PagingLinksBuilder.cs
==================

A single .cs file for you to add to your project. It quickly adds a very configurable paging control to your project. It renders out-of-the-box Twitter Bootstrap fiendly too!

![PagingLinksBuidler preview](https://raw.github.com/theonlylawislove/PagingLinksBuilder/master/preview.jpg)

# Getting Started

Add [the only file](https://raw.github.com/theonlylawislove/PagingLinksBuilder/master/PagingLinksBuilder.cs) you need to your project.

Add the following to an HtmlHelper extension class somewhere.

```c#
public static PagingLinksBuilder PagingLinksBuilder(this HtmlHelper helper, int currentPage, int totalPages, Func<int, string> pageUrlBuilder)
{
    return new PagingLinksBuilder(currentPage, totalPages, pageUrlBuilder);
}
```

You are ready to rock-n-roll!

#Configuration/Defaults

The following represents a complete configuration of defaults.