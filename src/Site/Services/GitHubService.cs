using System.Net;
using System.Net.Http.Json;
using Site.Models;

namespace Site.Services;

/// <summary>Résultat agrégé du chargement GitHub.</summary>
public sealed class GitHubData
{
    public GitHubUser? User { get; init; }
    public IReadOnlyList<GitHubRepo> Repos { get; init; } = [];
    public bool RateLimited { get; init; }
    public bool Failed { get; init; }
}

/// <summary>
/// Charge dynamiquement les dépôts publics du compte <c>74nu5</c> et de
/// l'organisation <c>Tanuscorp</c> depuis l'API REST GitHub (non authentifiée).
/// Résultat mis en cache pour la durée de vie de l'application.
/// </summary>
public sealed class GitHubService(HttpClient http)
{
    private const string User = "74nu5";
    private const string Org = "Tanuscorp";

    private GitHubData? _cache;
    private Task<GitHubData>? _inFlight;

    public Task<GitHubData> LoadAsync()
    {
        if (_cache is not null)
            return Task.FromResult(_cache);

        // Évite les appels concurrents multiples (plusieurs composants).
        return _inFlight ??= LoadCoreAsync();
    }

    private async Task<GitHubData> LoadCoreAsync()
    {
        try
        {
            var userTask = GetJsonAsync($"https://api.github.com/users/{User}", GitHubJsonContext.Default.GitHubUser);
            var userReposTask = GetReposAsync($"https://api.github.com/users/{User}/repos?per_page=100&sort=updated", "74nu5");
            var orgReposTask = GetReposAsync($"https://api.github.com/orgs/{Org}/repos?per_page=100&sort=updated", "Tanuscorp");

            await Task.WhenAll(userTask, userReposTask, orgReposTask);

            var user = userTask.Result;
            var repos = new List<GitHubRepo>();
            repos.AddRange(userReposTask.Result);
            repos.AddRange(orgReposTask.Result);

            var cleaned = repos
                .Where(r => r is { Private: false, Fork: false })
                .GroupBy(r => r.Id)
                .Select(g => g.First())
                .OrderByDescending(r => r.StargazersCount)
                .ThenByDescending(r => r.PushedAt ?? DateTimeOffset.MinValue)
                .ToList();

            _cache = new GitHubData
            {
                User = user,
                Repos = cleaned,
                RateLimited = false,
                Failed = false,
            };
            return _cache;
        }
        catch (RateLimitException)
        {
            _cache = new GitHubData { RateLimited = true };
            return _cache;
        }
        catch
        {
            // Échec réseau/parsing : on renvoie un état d'erreur sans re-cacher
            // définitivement pour permettre un nouvel essai ultérieur.
            _inFlight = null;
            return new GitHubData { Failed = true };
        }
    }

    private async Task<List<GitHubRepo>> GetReposAsync(string url, string ownerLabel)
    {
        var repos = await GetJsonAsync(url, GitHubJsonContext.Default.GitHubRepoArray) ?? [];
        foreach (var r in repos)
            r.OwnerLabel = ownerLabel;
        return [.. repos];
    }

    private async Task<T?> GetJsonAsync<T>(string url, System.Text.Json.Serialization.Metadata.JsonTypeInfo<T> typeInfo)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/vnd.github+json");

        using var response = await http.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Forbidden || (int)response.StatusCode == 429)
            throw new RateLimitException();

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(typeInfo);
    }

    private sealed class RateLimitException : Exception;
}
