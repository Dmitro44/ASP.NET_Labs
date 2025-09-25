namespace WEB_353503_Sebelev.UI.Services.Authentication;

public interface ITokenAccessor
{
    /// <summary>
    /// Добавление заголовка Authorization : bearer
    /// </summary>
    /// <param name="client">HttpClient, в который добавляется заголовок</param>
    /// <param name="isClient">если true - получить токен клиента; если false - получить токен пользователя</param>
    /// <returns></returns>
    Task SetAuthorizationHeaderAsync(HttpClient client, bool isClient);

}