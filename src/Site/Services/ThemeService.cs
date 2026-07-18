using Microsoft.JSInterop;

namespace Site.Services;

public enum Theme
{
    Dark,
    Light,
}

/// <summary>
/// Gère le thème clair/sombre : lecture/écriture dans <c>localStorage</c> et
/// application de l'attribut <c>data-theme</c> sur &lt;html&gt;. Le thème est
/// déjà appliqué très tôt par un script inline dans <c>index.html</c> ; ce
/// service synchronise ensuite l'état côté Blazor.
/// </summary>
public sealed class ThemeService(IJSRuntime js)
{
    public Theme Current { get; private set; } = Theme.Dark;

    public event Action? OnChanged;

    public bool IsDark => Current == Theme.Dark;

    public async Task InitializeAsync()
    {
        var stored = await js.InvokeAsync<string?>("siteInterop.getStorage", "theme");
        Current = stored switch
        {
            "light" => Theme.Light,
            "dark" => Theme.Dark,
            _ => await js.InvokeAsync<bool>("siteInterop.prefersLight") ? Theme.Light : Theme.Dark,
        };
        await ApplyAsync();
        OnChanged?.Invoke();
    }

    public async Task ToggleAsync()
    {
        Current = Current == Theme.Dark ? Theme.Light : Theme.Dark;
        await js.InvokeVoidAsync("siteInterop.setStorage", "theme", Current == Theme.Dark ? "dark" : "light");
        await ApplyAsync();
        OnChanged?.Invoke();
    }

    private ValueTask ApplyAsync() =>
        js.InvokeVoidAsync("siteInterop.applyTheme", Current == Theme.Dark ? "dark" : "light");
}
