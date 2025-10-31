using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WEB_353503_Sebelev.UI.TagHelpers;

[HtmlTargetElement("pager")]
public class PagerTagHelper : TagHelper
{
    /// <summary>
    /// Текущая страница
    /// </summary>
    [HtmlAttributeName("current-page")]
    public int CurrentPage { get; set; }

    /// <summary>
    /// Общее количество страниц
    /// </summary>
    [HtmlAttributeName("total-pages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Флаг администратора (true = Razor Page, false = MVC View)
    /// </summary>
    [HtmlAttributeName("admin")]
    public bool Admin { get; set; }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    private readonly LinkGenerator _linkGenerator;

    public PagerTagHelper(LinkGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        // Валидация
        if (TotalPages <= 0)
        {
            output.SuppressOutput();
            return;
        }

        CurrentPage = Math.Max(1, Math.Min(CurrentPage, TotalPages));

        // Устанавливаем основной тег
        output.TagName = "ul";
        output.Attributes.SetAttribute("class", "pagination mt-4");

        var content = new StringBuilder();

        // Кнопка "Предыдущая"
        content.Append("<li class=\"page-item");
        if (CurrentPage == 1)
            content.Append(" disabled");
        content.Append("\">");

        content.Append("<a class=\"page-link");
        if (CurrentPage == 1)
            content.Append(" disabled");
        content.Append("\" aria-label=\"Previous\" href=\"");
        
        if (CurrentPage > 1)
        {
            var url = GetPageUrl(CurrentPage - 1);
            content.Append(url);
        }
        else
        {
            content.Append("#");
        }

        content.Append("\"><span aria-hidden=\"true\">&laquo;</span></a></li>");

        // Номера страниц
        for (int i = 1; i <= TotalPages; i++)
        {
            content.Append("<li class=\"page-item");
            if (CurrentPage == i)
                content.Append(" active");
            content.Append("\">");

            content.Append("<a class=\"page-link\" href=\"");
            var url = GetPageUrl(i);
            content.Append(url);
            content.Append("\">");
            content.Append(i);
            content.Append("</a></li>");
        }

        // Кнопка "Следующая"
        content.Append("<li class=\"page-item");
        if (CurrentPage == TotalPages)
            content.Append(" disabled");
        content.Append("\">");

        content.Append("<a class=\"page-link");
        if (CurrentPage == TotalPages)
            content.Append(" disabled");
        content.Append("\" aria-label=\"Next\" href=\"");

        if (CurrentPage < TotalPages)
        {
            var url = GetPageUrl(CurrentPage + 1);
            content.Append(url);
        }
        else
        {
            content.Append("#");
        }

        content.Append("\"><span aria-hidden=\"true\">&raquo;</span></a></li>");

        output.Content.SetHtmlContent(content.ToString());
    }

    private string GetPageUrl(int pageNo)
    {
        var httpContext = ViewContext.HttpContext;
        var routeData = ViewContext.RouteData.Values;
        string url;

        if (Admin)
        {
            // Razor Page для админки - получаем текущий path
            var page = routeData["page"]?.ToString();
            url = _linkGenerator.GetPathByPage(
                httpContext,
                page,
                null,
                new { pageNo }) ?? "";
        }
        else
        {
            // MVC Controller/Action - получаем из текущего маршрута
            var controller = routeData["controller"]?.ToString() ?? "Home";
            var action = routeData["action"]?.ToString() ?? "Index";

            url = _linkGenerator.GetPathByAction(
                httpContext,
                action,
                controller,
                new { pageNo }) ?? "";
        }

        return url;
    }
}