namespace Site.Data;

/// <summary>Une application déployée en ligne (sous-domaine de 74nu5.dev).</summary>
public sealed record LiveApp(
    string Name,
    string Subdomain,
    string Url,
    string? RepoUrl,
    string DescFr,
    string DescEn,
    string Tech,
    string Icon);

/// <summary>Un lien de contact / réseau.</summary>
public sealed record ContactLink(string Label, string Sub, string Url, string Icon, bool External = true);

/// <summary>Un groupe de technologies pour la section « stack ».</summary>
public sealed record StackGroup(string TitleFr, string TitleEn, string[] Items);

/// <summary>
/// Contenu curé du site (données non découvrables via l'API GitHub).
/// Modifiable librement — c'est ici qu'on ajuste textes, apps et liens.
/// </summary>
public static class SiteContent
{
    public const string Name = "tanus";
    public const string Handle = "74nu5";
    public const string Location = "Nantes, France";
    public const string Company = "@3bstudio";
    public const string AvatarUrl = "https://avatars.githubusercontent.com/u/325077?v=4";
    public const string GitHubUrl = "https://github.com/74nu5";
    public const string GitHubOrgUrl = "https://github.com/Tanuscorp";
    public const string Email = "info@tanus.eu";
    public const string SiteRepoUrl = "https://github.com/74nu5/74nu5";

    /// <summary>Rôles défilants dans le hero, par langue.</summary>
    public static string[] Roles(bool fr) => fr
        ? ["développeur .NET", "Blazor & Web", "outils pour devs", "applications desktop", "open source"]
        : [".NET developer", "Blazor & Web", "developer tooling", "desktop apps", "open source"];

    public static readonly LiveApp[] Apps =
    [
        new("LINQ Marbles", "linq-marbles.74nu5.dev", "https://linq-marbles.74nu5.dev",
            "https://github.com/74nu5/LINQMarbles",
            "Diagrammes « marble » interactifs pour apprendre et expérimenter les opérateurs LINQ.",
            "Interactive marble diagrams to learn and experiment with LINQ operators.",
            "Web · Interactive", "diagram"),

        new("MD → LinkedIn", "md-to-linkedin.74nu5.dev", "https://md-to-linkedin.74nu5.dev",
            "https://github.com/74nu5/MdToLinkedIn",
            "Convertit du Markdown en texte formaté prêt à coller sur LinkedIn (gras, italique, listes Unicode).",
            "Converts Markdown into LinkedIn-ready formatted text (bold, italic, Unicode lists).",
            "Web · Tool", "text"),

        new("Copy to NAS", "copy-to-nas.74nu5.dev", "https://copy-to-nas.74nu5.dev",
            "https://github.com/74nu5/CopyToNas",
            "Transfert de fichiers depuis des serveurs SFTP vers ta machine, en CLI ou via une UI web Blazor : progression en temps réel (vitesse, ETA), copie récursive et connexions SFTP sécurisées.",
            "Copy files and directories from SFTP servers to your machine — CLI or Blazor web UI, with real-time progress (speed, ETA), recursive copy and secure SFTP connections.",
            "SFTP · Blazor · CLI", "server"),

        new("Hardlink Analyzer", "hardlink-analyzer.74nu5.dev", "https://hardlink-analyzer.74nu5.dev",
            "https://github.com/74nu5/ScriptServer",
            "Application .NET 10 multiplateforme qui repère les fichiers identiques (SHA256) et crée des liens physiques pour économiser de l'espace disque sans dupliquer les données. Mode simulation (--dry-run).",
            ".NET 10 cross-platform tool that finds identical files (SHA256) and creates hard links to save disk space without duplicating data — includes a --dry-run simulation mode.",
            "CLI · .NET 10 · Cross-platform", "link"),
    ];

    public static readonly ContactLink[] Contacts =
    [
        new("GitHub", "@74nu5", GitHubUrl, "github"),
        new("Organisation", "@Tanuscorp", GitHubOrgUrl, "org"),
        new("Email", Email, $"mailto:{Email}", "mail"),
    ];

    public static readonly StackGroup[] Stack =
    [
        new("Langages", "Languages", ["C#", "TypeScript", "JavaScript", "HTML", "CSS / Sass"]),
        new(".NET", ".NET", ["ASP.NET Core", "Blazor", "WebAssembly", "Entity Framework", "WinUI", ".NET MAUI", "CLI tools"]),
        new("Outils", "Tooling", ["Visual Studio", "VS Code", "Git", "GitHub Actions", "Azure", "Docker"]),
        new("Centres d'intérêt", "Focus areas", ["Outils dev", "Diagrammes / Mermaid", "Automatisation", "Desktop apps"]),
    ];
}
