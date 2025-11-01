using System.Text.Json;
using WEB_353503_Sebelev.Domain.Entities;

namespace WEB_353503_Sebelev.UI.Extensions;

public static class SessionExtensions
{
    public static T? Get<T>(this ISession session, string key) where T : class
    {
        var value = session.GetString(key);

        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(value);
        }
        catch
        {
            return null;
        }
    }

    public static void Set<T>(this ISession session, string key, T value) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        session.SetString(key, json);
    }
}