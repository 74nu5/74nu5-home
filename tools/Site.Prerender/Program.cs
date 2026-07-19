using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

using Site.Layout;
using Site.Pages;
using Site.Services;

// Prérendu à la build : rend la page d'accueil en HTML statique et l'injecte
// dans l'index.html publié, pour que les crawlers voient le contenu réel.
// Aucune dépendance tierce : uniquement le HtmlRenderer du framework.

const string StartMarker = "<!--PRERENDER-->";
const string EndMarker = "<!--/PRERENDER-->";

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: Site.Prerender <chemin du wwwroot publié>");
    return 1;
}

var wwwroot = args[0];
var indexPath = Path.Combine(wwwroot, "index.html");

if (!File.Exists(indexPath))
{
    Console.Error.WriteLine($"index.html introuvable : {indexPath}");
    return 1;
}

var services = new ServiceCollection();
services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Warning));
services.AddSingleton<IJSRuntime, NoOpJsRuntime>();
services.AddScoped(_ => CreateGitHubClient());
services.AddScoped<GitHubService>();
services.AddScoped<LocalizationService>();
services.AddScoped<ThemeService>();

await using var provider = services.BuildServiceProvider();
var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

await using var renderer = new HtmlRenderer(provider, loggerFactory);

var html = await renderer.Dispatcher.InvokeAsync(async () =>
{
    // MainLayout attend son contenu via le paramètre Body (LayoutComponentBase).
    RenderFragment body = builder =>
    {
        builder.OpenComponent<Home>(0);
        builder.CloseComponent();
    };

    var parameters = ParameterView.FromDictionary(new Dictionary<string, object?> { ["Body"] = body });
    var output = await renderer.RenderComponentAsync<MainLayout>(parameters);
    return output.ToHtmlString();
});

var index = await File.ReadAllTextAsync(indexPath);
var start = index.IndexOf(StartMarker, StringComparison.Ordinal);
var end = index.IndexOf(EndMarker, StringComparison.Ordinal);

if (start < 0 || end < 0 || end < start)
{
    Console.Error.WriteLine($"Marqueurs {StartMarker} / {EndMarker} absents de index.html — prérendu annulé.");
    return 1;
}

var updated = index[..(start + StartMarker.Length)] + "\n" + html + "\n" + index[end..];

await File.WriteAllTextAsync(indexPath, updated);

Console.WriteLine($"Prérendu injecté dans {indexPath} ({html.Length:N0} caractères de HTML).");
return 0;

// L'API GitHub exige un User-Agent côté serveur (contrairement au navigateur).
// GITHUB_TOKEN (optionnel) relève la limite de 60 à 5000 requêtes/heure.
static HttpClient CreateGitHubClient()
{
    var http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
    http.DefaultRequestHeaders.UserAgent.ParseAdd("74nu5-home-prerender/1.0");

    var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
    if (!string.IsNullOrWhiteSpace(token))
        http.DefaultRequestHeaders.Authorization = new("Bearer", token);

    return http;
}

/// <summary>
/// Le rendu statique n'exécute jamais OnAfterRender, donc aucun appel JS n'a
/// lieu. Ce substitut satisfait simplement l'injection de dépendances.
/// </summary>
internal sealed class NoOpJsRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        => ValueTask.FromResult<TValue>(default!);

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => ValueTask.FromResult<TValue>(default!);
}
