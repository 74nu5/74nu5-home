using System.Text.Json.Serialization;

namespace Site.Models;

/// <summary>Un dépôt GitHub tel que renvoyé par l'API REST v3.</summary>
public sealed class GitHubRepo
{
    public long Id { get; set; }
    public string Name { get; set; } = "";
    public string FullName { get; set; } = "";
    public string? Description { get; set; }
    public string HtmlUrl { get; set; } = "";
    public string? Homepage { get; set; }
    public string? Language { get; set; }
    public int StargazersCount { get; set; }
    public int ForksCount { get; set; }
    public bool Fork { get; set; }
    public bool Archived { get; set; }
    public bool Private { get; set; }
    public string[]? Topics { get; set; }
    public DateTimeOffset? PushedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public GitHubLicense? License { get; set; }

    /// <summary>Compte (74nu5 ou Tanuscorp) — rempli côté client, pas par l'API.</summary>
    [JsonIgnore]
    public string OwnerLabel { get; set; } = "";
}

public sealed class GitHubLicense
{
    public string? SpdxId { get; set; }
    public string? Name { get; set; }
}

/// <summary>Profil public GitHub.</summary>
public sealed class GitHubUser
{
    public string Login { get; set; } = "";
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? Company { get; set; }
    public string? Location { get; set; }
    public string? Blog { get; set; }
    public string AvatarUrl { get; set; } = "";
    public string HtmlUrl { get; set; } = "";
    public int Followers { get; set; }
    public int Following { get; set; }
    public int PublicRepos { get; set; }
}

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(GitHubRepo[]))]
[JsonSerializable(typeof(List<GitHubRepo>))]
[JsonSerializable(typeof(GitHubRepo))]
[JsonSerializable(typeof(GitHubUser))]
[JsonSerializable(typeof(GitHubLicense))]
public partial class GitHubJsonContext : JsonSerializerContext
{
}
