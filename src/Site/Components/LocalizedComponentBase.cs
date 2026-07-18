using Microsoft.AspNetCore.Components;
using Site.Services;

namespace Site.Components;

/// <summary>
/// Base pour les composants dépendant de la langue et du thème : ré-affiche
/// automatiquement le composant quand l'un ou l'autre change.
/// </summary>
public abstract class LocalizedComponentBase : ComponentBase, IDisposable
{
    [Inject] protected LocalizationService Loc { get; set; } = default!;
    [Inject] protected ThemeService Theme { get; set; } = default!;

    protected override void OnInitialized()
    {
        Loc.OnChanged += HandleChanged;
        Theme.OnChanged += HandleChanged;
    }

    private void HandleChanged() => _ = InvokeAsync(StateHasChanged);

    public virtual void Dispose()
    {
        Loc.OnChanged -= HandleChanged;
        Theme.OnChanged -= HandleChanged;
        GC.SuppressFinalize(this);
    }
}
