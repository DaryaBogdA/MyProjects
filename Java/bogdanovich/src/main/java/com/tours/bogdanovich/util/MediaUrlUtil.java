package com.tours.bogdanovich.util;

public final class MediaUrlUtil {

    private static final String DEFAULT_TOUR_IMAGE =
            "https://images.unsplash.com/photo-1488646953014-85cb44e25828?auto=format&fit=crop&w=800&q=80";

    private MediaUrlUtil() {
    }

    public static String normalizePhotoUrl(String photoUrl) {
        if (photoUrl == null || photoUrl.isBlank()) {
            return DEFAULT_TOUR_IMAGE;
        }
        String url = photoUrl.trim();
        if (url.startsWith("http://") || url.startsWith("https://")) {
            return url;
        }
        if (url.startsWith("/api/assets/") || url.startsWith("/api/uploads/")) {
            return url;
        }
        if (url.startsWith("../static/")) {
            return "/api/assets/" + url.substring("../static/".length());
        }
        if (url.startsWith("/static/")) {
            return "/api/assets/" + url.substring("/static/".length());
        }
        if (url.startsWith("static/")) {
            return "/api/assets/" + url.substring("static/".length());
        }
        if (url.startsWith("img/")) {
            return "/api/assets/" + url;
        }
        if (!url.startsWith("/")) {
            return "/api/assets/img/countries/" + url;
        }
        return url;
    }
}
