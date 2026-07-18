# 74nu5.dev

Site personnel / portfolio de **tanus** ([@74nu5](https://github.com/74nu5)) — construit en **Blazor WebAssembly (.NET 10)**, statique, bilingue (FR/EN), thème sombre/clair.

> Personal portfolio site — Blazor WebAssembly (.NET 10), fully static, bilingual (FR/EN), dark/light theme.

## ✨ Fonctionnalités

- **Applications en ligne** — les outils déployés sur les sous-domaines de `74nu5.dev`.
- **Projets open source** — chargés **dynamiquement** depuis l'API GitHub (`@74nu5` + `@Tanuscorp`), avec recherche, tri et filtre par compte (forks exclus).
- **Bilingue FR/EN** — bascule instantanée, préférence mémorisée (`localStorage`).
- **Thème sombre / clair** — appliqué avant le rendu (aucun flash), mémorisé.
- **Zéro dépendance JS externe** — CSS + SVG maison, un seul petit fichier d'interop.

## 🧱 Stack

- .NET 10 · Blazor WebAssembly (standalone)
- `System.Text.Json` avec **source generation** (compatible trimming)
- Design system CSS custom (variables, responsive, `prefers-color-scheme`)

## 🚀 Développement

```bash
# Lancer en local
dotnet run --project src/Site

# Build de production
dotnet publish src/Site/Site.csproj -c Release -o publish
# → les fichiers statiques sont dans publish/wwwroot
```

## ☁️ Déploiement — Azure Static Web Apps

Le déploiement est automatisé via GitHub Actions : [`.github/workflows/azure-static-web-apps.yml`](.github/workflows/azure-static-web-apps.yml).

Étapes pour brancher le déploiement :

1. Pousser ce dépôt sur GitHub.
2. Créer une ressource **Azure Static Web App** (plan *Free* suffit), source **GitHub**.
   - App location : `publish/wwwroot` · Output location : *(vide)* · Api : *(vide)*
   - Ou : source **Other** puis récupérer le token de déploiement.
3. Ajouter le secret `AZURE_STATIC_WEB_APPS_API_TOKEN` dans les secrets Actions du dépôt.
4. Configurer le domaine personnalisé `74nu5.dev` dans le portail Azure.

Le fichier [`src/Site/wwwroot/staticwebapp.config.json`](src/Site/wwwroot/staticwebapp.config.json) gère le *fallback* SPA (toutes les routes renvoient `index.html`).

## 📝 Personnalisation du contenu

Tout le contenu curé (apps, liens de contact, stack, textes bilingues) est centralisé :

- [`src/Site/Data/SiteContent.cs`](src/Site/Data/SiteContent.cs) — apps, liens, technologies, profil.
- [`src/Site/Services/LocalizationService.cs`](src/Site/Services/LocalizationService.cs) — toutes les traductions FR/EN.

Les dépôts GitHub, eux, n'ont **rien à maintenir** : ils sont récupérés à chaud.
