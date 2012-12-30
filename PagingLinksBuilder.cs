using System;
using System.Web;
using System.Web.WebPages;

/// <summary>
/// Builds paging links for a given pageNumber and totalPages in a fully configurable fashion.
/// Built by Paul Knopf (pknopf.com)
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagingLinksBuilder<T> : IHtmlString
{
    #region Fields

    // paging info
    private readonly int _currentPage;
    private readonly int _totalPages;
    // layout
    private Func<HelperResult, HelperResult> _layoutTemplate;
    private Func<RenderLinkModel, HelperResult> _linkTemplate;
    private Func<RenderItemModel, HelperResult> _itemTemplate;
    // misc
    private Func<int, string> _pageUrlBuilder;
    private bool _alwaysShowNavigation;
    private int _maxNumberOfTrailingLeadingPages;
    // links
    private Func<int, HelperResult> _pageLink;
    private Func<int, HelperResult> _nextLink;
    private Func<int, HelperResult> _previousLink;
    private Func<int, HelperResult> _firstLink;
    private Func<int, HelperResult> _lastLink;
    // css classes
    private string _previousCssClass;
    private string _nextCssClass;
    private string _firstCssClass;
    private string _lastCssClass;
    private string _activeCssClass;
    private string _pageCssClass;
    private string _disabledCssClass;

    #endregion


    #region Enums

    /// <summary>
    /// The type of link being rendered.
    /// </summary>
    public enum LinkType
    {
        First,
        Previous,
        Page,
        Next,
        Last
    }

    #endregion

    #region Ctor

    /// <summary>
    /// Initializes a new PagingLinksBuilder
    /// </summary>
    /// <param name="totalPages"></param>
    /// <param name="pageUrlBuilder">The delegate that will be used to be page urls, for example: /category?page=1</param>
    /// <param name="currentPage"></param>
    public PagingLinksBuilder(int currentPage, int totalPages, Func<int, string> pageUrlBuilder)
        : base()
    {
        _currentPage = currentPage;
        _totalPages = totalPages;
        _pageUrlBuilder = pageUrlBuilder;
        InitializeDefaults();
    }

    #endregion

    #region Helpers

    #region Layout

    /// <summary>
    /// Set the layout template to use.
    /// -------------
    /// Default
    /// -------------
    /// <div class="pagination">
    ///     <ul>@item</ul>
    /// </div>
    /// -------------
    /// </summary>
    /// <param name="layoutTemplate"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> LayoutTemplate(Func<HelperResult, HelperResult> layoutTemplate)
    {
        _layoutTemplate = layoutTemplate;
        return this;
    }

    /// <summary>
    /// Set the item template to use.
    /// -------------
    /// Default
    /// -------------
    /// <li class="@item.CssClass">@item.LinkRender</li>
    /// -------------
    /// </summary>
    /// <param name="itemTempate"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> ItemTemplate(Func<RenderItemModel, HelperResult> itemTempate)
    {
        _itemTemplate = itemTempate;
        return this;
    }

    /// <summary>
    /// Set the link template to use.
    /// -------------
    /// Default
    /// -------------
    /// <a href="@item.GetPageUrl()" class="@item.CssClass">@item.LinkText</a>
    /// -------------
    /// </summary>
    /// <param name="linkTemplate"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> LinkTemplate(Func<RenderLinkModel, HelperResult> linkTemplate)
    {
        _linkTemplate = linkTemplate;
        return this;
    }

    #endregion

    #region Misc

    /// <summary>
    /// The first/previous/next/last links will be hidden if they are not valid.
    /// This bool will allow you to always render them, 
    /// but render them disabled (javascript:void() with disabledCssClass).
    /// </summary>
    /// <param name="alwaysShowNavigation"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> AlwaysShowNavigation(bool alwaysShowNavigation)
    {
        _alwaysShowNavigation = alwaysShowNavigation;
        return this;
    }

    /// <summary>
    /// Set the page-url builder that will be invoked when rendering pieces need a url to a page.
    /// </summary>
    /// <param name="pageUrlBuilder"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> PageUrlBuilder(Func<int, string> pageUrlBuilder)
    {
        _pageUrlBuilder = pageUrlBuilder;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of trailing/leading pages.
    /// For example: 
    ///     MaxNumberOfTrailingLeadingPages(2);
    ///         [1] -  2  -  3  -  4  -  5
    ///          3  -  4  - [5] -  6  -  7
    ///          5  -  6  -  7  -  8  - [9]
    ///     MaxNumberOfTrailingLeadingPages(3);
    ///         [1] -  2  -  3  -  4  -  5  -  6  -  7
    ///          2  -  3  -  4  - [5] -  6  -  7  -  8
    ///          3  -  4  -  5  -  6  -  7  -  8  - [9] 
    /// </summary>
    /// <param name="maxNumberOfTrailingLeadingPages"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> MaxNumberOfTrailingLeadingPages(int maxNumberOfTrailingLeadingPages)
    {
        _maxNumberOfTrailingLeadingPages = maxNumberOfTrailingLeadingPages;
        return this;
    }

    #endregion

    #region CssClasses

    /// <summary>
    /// Set the \"previous\" css class
    /// </summary>
    /// <param name="previousCssClass"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> PreviousCssClass(string previousCssClass)
    {
        _previousCssClass = previousCssClass;
        return this;
    }

    /// <summary>
    /// Set the \"next\" css class
    /// </summary>
    /// <param name="nextCssClass"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> NextCssClass(string nextCssClass)
    {
        _nextCssClass = nextCssClass;
        return this;
    }

    /// <summary>
    /// Set the \"last\" css class
    /// </summary>
    /// <param name="lastCssClass"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> LastCssClass(string lastCssClass)
    {
        _lastCssClass = lastCssClass;
        return this;
    }

    /// <summary>
    /// Set the \"first\" css class
    /// </summary>
    /// <param name="firstCssClass"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> FirstCssClass(string firstCssClass)
    {
        _firstCssClass = firstCssClass;
        return this;
    }

    /// <summary>
    /// Set the \"page\" css class
    /// </summary>
    /// <param name="pageCssClass"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> PageCssClass(string pageCssClass)
    {
        _pageCssClass = pageCssClass;
        return this;
    }

    /// <summary>
    /// Set the \"disabled\" css class
    /// </summary>
    /// <param name="disabledCssClass"></param>
    /// <returns></returns>
    public PagingLinksBuilder<T> DisabledCssClass(string disabledCssClass)
    {
        _disabledCssClass = disabledCssClass;
        return this;
    }

    #endregion

    #endregion

    #region Private

    /// <summary>
    /// Initialize all the default values.
    /// </summary>
    private void InitializeDefaults()
    {
        _layoutTemplate = (items) => new HelperResult(writer =>
        {
            writer.Write("<div class=\"pagination\"><ul>");
            items.WriteTo(writer);
            writer.Write("</ul></div>");
        });
        _itemTemplate = (model) => new HelperResult(writer =>
        {
            writer.Write("<li class=\"" + model.CssClass + "\">");
            model.LinkRender.WriteTo(writer);
            writer.Write("</li>");
        });
        _linkTemplate = (model) => new HelperResult(writer =>
        {
            writer.Write("<a href=\"" + model.GetPageUrl() + "\" class=\"" + model.CssClass + "\">");
            writer.Write(model.LinkText);
            writer.Write("</a>");
        });

        _maxNumberOfTrailingLeadingPages = 2;
        _alwaysShowNavigation = true;

        _pageLink = (pageNumber) => BuildItem(pageNumber.ToString(), pageNumber, LinkType.Page);
        _nextLink = (pageNumber) => BuildItem("Next", pageNumber, LinkType.Next);
        _previousLink = (pageNumber) => BuildItem("Previous", pageNumber, LinkType.Previous);
        _firstLink = (pageNumber) => BuildItem("First", pageNumber, LinkType.First);
        _lastLink = (pageNumber) => BuildItem("Last", pageNumber, LinkType.Last);

        _firstCssClass = "first";
        _previousCssClass = "previous";
        _nextCssClass = "next";
        _lastCssClass = "last";
        _pageCssClass = "page";
        _disabledCssClass = "disabled";
        _activeCssClass = "active";
    }

    /// <summary>
    /// Builds an item (and the link nested within it)
    /// </summary>
    /// <returns></returns>
    private HelperResult BuildItem(string text, int pageNumber, LinkType linkType)
    {
        // build the item model
        var linkModel = new RenderLinkModel();
        linkModel.PageNumber = pageNumber;
        linkModel.CurrentPage = _currentPage;
        linkModel.LinkType = linkType;
        linkModel.LinkText = text;
        linkModel.PageUrlBuilder = _pageUrlBuilder;

        switch (linkType)
        {
            case LinkType.First:
                linkModel.CssClass = _firstCssClass;
                break;
            case LinkType.Previous:
                linkModel.CssClass = _previousCssClass;
                break;
            case LinkType.Page:
                linkModel.CssClass = _pageCssClass;
                if (linkModel.Disabled)
                    // it is disabled because it is selected and active!
                    linkModel.CssClass = (linkModel.CssClass + " " + _activeCssClass).Trim();
                break;
            case LinkType.Next:
                linkModel.CssClass = _nextCssClass;
                break;
            case LinkType.Last:
                linkModel.CssClass = _lastCssClass;
                break;
        }

        if (linkModel.Disabled)
            linkModel.CssClass = (linkModel.CssClass + " " + _disabledCssClass).Trim();

        var itemModel = new RenderItemModel();
        itemModel.PageNumber = pageNumber;
        itemModel.CurrentPage = _currentPage;
        itemModel.LinkType = linkType;
        itemModel.CssClass = linkModel.CssClass;
        itemModel.LinkRender = new HelperResult(writer => writer.Write(_linkTemplate(linkModel)));

        return _itemTemplate(itemModel);
    }

    /// <summary>
    /// Builds the 'nearby' pages (21, 22, 23, 24, 25), etc.
    /// </summary>
    /// <param name="action"></param>
    private void BuildPageRange(Action<int> action)
    {
        var min = _currentPage - _maxNumberOfTrailingLeadingPages;
        var max = _currentPage + _maxNumberOfTrailingLeadingPages;


        if (min < 1)
        {
            // push the '5' visible links to the right so that it starts at '1';
            var offset = 1 - min;
            min += offset;
            max += offset;
        }

        if (max > _totalPages)
        {
            // push the '5' visible links to the left so that it ends at 'totalpages';
            var offset = max - _totalPages;
            min -= offset;
            max -= offset;
        }

        for (var x = min; x <= max; x++)
        {
            // only render the page if it is between 1 and totalPages
            if (x > 0 && x <= _totalPages)
                action.Invoke(x);
        }
    }

    #endregion

    #region IHtmlString

    public string ToHtmlString()
    {
        var items = new HelperResult(writer =>
        {
            // render the first/previous if needed be
            if (_alwaysShowNavigation ||
                (_totalPages > 1 && _currentPage != 1))
            {
                writer.Write(_firstLink(1));
                writer.Write(_previousLink(Math.Max(1, _currentPage - 1)));
            }

            // build the page number looks 'around' the curent page
            BuildPageRange((pageNumber) => writer.Write(_pageLink(pageNumber)));

            // render the next/last if needed be
            if (_alwaysShowNavigation ||
                (_currentPage < _totalPages))
            {
                writer.Write(_nextLink(Math.Min(_currentPage + 1, _totalPages)));
                writer.Write(_lastLink(_totalPages));
            }
        });

        return _layoutTemplate(items).ToHtmlString();
    }

    #endregion

    #region NestedClasses

    /// <summary>
    /// Base model information shared between item templates and link templates.
    /// </summary>
    public abstract class BaseRenderItemModel
    {
        /// <summary>
        /// The link type of the link to be rendered (first, previous,page, next, last).
        /// </summary>
        public LinkType LinkType { get; set; }

        /// <summary>
        /// The page number of the link we are rendering
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The currently selected/visited page
        /// </summary>
        public int CurrentPage { get; set; }

        /// <summary>
        /// The css class to be added to the item or link
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// If we are rendering a link for a page that is currently being viewed, 
        /// then the link will be displayed as disabled because the user is currently on that page.
        /// </summary>
        public bool Disabled { get { return PageNumber == CurrentPage; } }
    }

    /// <summary>
    /// This is the model that is given to each HelperResult to render each item
    /// </summary>
    public class RenderItemModel : BaseRenderItemModel
    {
        /// <summary>
        /// The rendering of the inner link. 
        /// This must be used in the item template or the links will not render.
        /// </summary>
        public HelperResult LinkRender { get; set; }
    }

    /// <summary>
    /// This is the model that is given to each HelperResult to render a link
    /// </summary>
    public class RenderLinkModel : RenderItemModel
    {
        /// <summary>
        /// Builds a url for the given page.
        /// </summary>
        public Func<int, string> PageUrlBuilder { get; set; }

        /// <summary>
        /// The text to be displayed inside of the link
        /// </summary>
        public string LinkText { get; set; }

        /// <summary>
        /// Gets this links page url
        /// </summary>
        /// <returns></returns>
        public string GetPageUrl()
        {
            return Disabled ? "javascript:void(0);" : PageUrlBuilder(PageNumber);
        }
    }

    #endregion
}