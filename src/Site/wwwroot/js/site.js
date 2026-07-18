// Interop léger pour le site — pas de dépendance externe.
window.siteInterop = {
    getStorage: function (key) {
        try { return localStorage.getItem(key); } catch (e) { return null; }
    },
    setStorage: function (key, value) {
        try { localStorage.setItem(key, value); } catch (e) { }
    },
    applyTheme: function (theme) {
        document.documentElement.setAttribute('data-theme', theme);
    },
    applyLang: function (lang) {
        document.documentElement.setAttribute('lang', lang);
    },
    prefersLight: function () {
        try { return window.matchMedia('(prefers-color-scheme: light)').matches; } catch (e) { return false; }
    },
    scrollToId: function (id) {
        var el = document.getElementById(id);
        if (el) { el.scrollIntoView({ behavior: 'smooth', block: 'start' }); }
    }
};
