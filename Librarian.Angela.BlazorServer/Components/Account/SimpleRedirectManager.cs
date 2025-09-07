using Microsoft.AspNetCore.Components;

namespace Librarian.Angela.BlazorServer.Components.Account;

public class SimpleRedirectManager
{
    private readonly NavigationManager _navigationManager;

    public SimpleRedirectManager(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public void RedirectTo(string? uri)
    {
        uri ??= "";
        _navigationManager.NavigateTo(uri);
    }

    public void RedirectTo(string uri, Dictionary<string, object?> queryParameters)
    {
        var uriWithQuery = _navigationManager.GetUriWithQueryParameters(uri, queryParameters);
        _navigationManager.NavigateTo(uriWithQuery);
    }
}