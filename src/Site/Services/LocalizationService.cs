using Microsoft.JSInterop;

namespace Site.Services;

public enum Lang
{
    Fr,
    En,
}

/// <summary>
/// Localisation FR/EN minimaliste et sans dépendance : un dictionnaire clé →
/// (fr, en). La langue est persistée dans <c>localStorage</c> et notifiée aux
/// composants abonnés.
/// </summary>
public sealed class LocalizationService(IJSRuntime js)
{
    public Lang Current { get; private set; } = Lang.Fr;

    public event Action? OnChanged;

    public bool IsFr => Current == Lang.Fr;

    /// <summary>Résout une paire (fr, en) selon la langue courante.</summary>
    public string Pick(string fr, string en) => Current == Lang.Fr ? fr : en;

    /// <summary>Indexeur de traduction : renvoie la clé si absente.</summary>
    public string this[string key] =>
        Strings.TryGetValue(key, out var pair) ? (Current == Lang.Fr ? pair.Fr : pair.En) : key;

    public async Task InitializeAsync()
    {
        var stored = await js.InvokeAsync<string?>("siteInterop.getStorage", "lang");
        Current = stored == "en" ? Lang.En : Lang.Fr;
        await js.InvokeVoidAsync("siteInterop.applyLang", Current == Lang.Fr ? "fr" : "en");
        OnChanged?.Invoke();
    }

    public async Task SetAsync(Lang lang)
    {
        if (lang == Current)
            return;

        Current = lang;
        await js.InvokeVoidAsync("siteInterop.setStorage", "lang", lang == Lang.Fr ? "fr" : "en");
        await js.InvokeVoidAsync("siteInterop.applyLang", lang == Lang.Fr ? "fr" : "en");
        OnChanged?.Invoke();
    }

    public Task ToggleAsync() => SetAsync(Current == Lang.Fr ? Lang.En : Lang.Fr);

    private static readonly Dictionary<string, (string Fr, string En)> Strings = new()
    {
        // Navigation
        ["nav.apps"] = ("apps", "apps"),
        ["nav.projects"] = ("projets", "projects"),
        ["nav.stack"] = ("stack", "stack"),
        ["nav.contact"] = ("contact", "contact"),

        // Hero
        ["hero.eyebrow"] = ("// développeur .NET · Nantes", "// .NET developer · Nantes"),
        ["hero.hello"] = ("Salut, moi c'est", "Hi, I'm"),
        ["hero.desc"] = (
            "Je conçois des applications web et desktop en .NET, des outils pour développeurs et quelques expérimentations. Voici un aperçu de ce que je construis — en ligne et open source.",
            "I build web & desktop apps in .NET, developer tooling and a few experiments. Here's a look at what I make — live and open source."),
        ["hero.cta_projects"] = ("Explorer les projets", "Explore projects"),
        ["hero.cta_github"] = ("GitHub", "GitHub"),

        // Stats
        ["stats.repos"] = ("dépôts publics", "public repos"),
        ["stats.followers"] = ("abonnés", "followers"),
        ["stats.apps"] = ("apps en ligne", "live apps"),
        ["stats.stars"] = ("étoiles cumulées", "total stars"),

        // Apps
        ["apps.index"] = ("01", "01"),
        ["apps.title"] = ("Applications en ligne", "Live apps"),
        ["apps.sub"] = (
            "Des outils que j'ai construits et déployés sur des sous-domaines de 74nu5.dev.",
            "Tools I built and deployed on 74nu5.dev subdomains."),
        ["apps.open"] = ("Ouvrir", "Open"),
        ["apps.source"] = ("Code", "Source"),

        // Projects
        ["projects.index"] = ("02", "02"),
        ["projects.title"] = ("Projets open source", "Open source projects"),
        ["projects.sub"] = (
            "Chargés en direct depuis GitHub (@74nu5 + @Tanuscorp), forks exclus.",
            "Loaded live from GitHub (@74nu5 + @Tanuscorp), forks excluded."),
        ["projects.search"] = ("filtrer les dépôts…", "filter repositories…"),
        ["projects.sort"] = ("Trier :", "Sort:"),
        ["projects.sort_stars"] = ("Étoiles", "Stars"),
        ["projects.sort_recent"] = ("Récents", "Recent"),
        ["projects.sort_name"] = ("Nom", "Name"),
        ["projects.all"] = ("Tous", "All"),
        ["projects.loading"] = ("récupération des dépôts…", "fetching repositories…"),
        ["projects.error"] = (
            "Impossible de charger les dépôts pour le moment.",
            "Couldn't load repositories right now."),
        ["projects.ratelimited"] = (
            "Limite de l'API GitHub atteinte. Réessaie dans quelques minutes.",
            "GitHub API rate limit reached. Please try again in a few minutes."),
        ["projects.empty"] = ("Aucun dépôt ne correspond à ce filtre.", "No repository matches this filter."),
        ["projects.viewall"] = ("Voir tout sur GitHub", "View all on GitHub"),
        ["projects.showing"] = ("dépôts affichés", "repositories shown"),

        // Stack
        ["stack.index"] = ("03", "03"),
        ["stack.title"] = ("Stack & outils", "Stack & tooling"),
        ["stack.sub"] = ("Ce avec quoi je travaille au quotidien.", "What I work with day to day."),

        // Contact
        ["contact.index"] = ("04", "04"),
        ["contact.title"] = ("Me contacter", "Get in touch"),
        ["contact.sub"] = (
            "Un projet, une question, ou juste envie d'échanger ? Voici où me trouver.",
            "A project, a question, or just want to chat? Here's where to find me."),

        // Footer
        ["footer.built"] = (
            "Conçu avec Blazor WebAssembly · .NET 10",
            "Built with Blazor WebAssembly · .NET 10"),
        ["footer.source"] = ("Code source", "Source code"),

        // 404
        ["nf.title"] = ("Page introuvable", "Page not found"),
        ["nf.desc"] = (
            "Cette route n'existe pas. Retour à la base.",
            "This route doesn't exist. Let's head back home."),
        ["nf.home"] = ("Retour à l'accueil", "Back home"),

        // Accessibilité
        ["a11y.theme"] = ("Changer de thème", "Toggle theme"),
        ["a11y.lang"] = ("Changer de langue", "Switch language"),
        ["a11y.menu"] = ("Ouvrir le menu", "Open menu"),
    };
}
