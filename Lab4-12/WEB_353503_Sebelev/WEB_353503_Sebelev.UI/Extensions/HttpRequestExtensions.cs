namespace WEB_353503_Sebelev.UI.Extensions;

public static class HttpRequestExtensions
{
    public static bool IsAjaxRequest(this HttpRequest request)
    {
        if (request.Headers.TryGetValue("x-requested-with", out var headerValue))
        {
            var value = headerValue.ToString();
            var result = !string.IsNullOrEmpty(value) && value.Contains("XMLHttpRequest");
            
            return result;
        }

        return false;
    }
}