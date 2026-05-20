(function () {
    var origin = window.location.origin;
    if (window.location.protocol === 'file:' || !origin || origin === 'null') {
        origin = 'http://localhost:8087';
    }
    var base = origin + '/api';
    window.API_BASE = base;
    window.SITE_BASE = base + '/site/';
    window.ASSETS_BASE = base + '/assets/';
})();
