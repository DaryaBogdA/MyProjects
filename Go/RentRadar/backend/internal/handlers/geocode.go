package handlers

import (
	"encoding/json"
	"io"
	"net/http"
	"net/url"
	"strconv"
	"strings"
	"time"
)

func (h *ListingHandler) GeocodeQuery(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	q := strings.TrimSpace(r.URL.Query().Get("q"))
	if len(q) < 4 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Слишком короткий запрос"})
		return
	}

	u := "https://nominatim.openstreetmap.org/search?q=" + url.QueryEscape(q) +
		"&format=json&limit=1&countrycodes=by"
	reqOut, err := http.NewRequestWithContext(r.Context(), http.MethodGet, u, nil)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	reqOut.Header.Set("User-Agent", "RentRadar/1.0 (listing geocode)")
	reqOut.Header.Set("Accept-Language", "ru")

	client := &http.Client{Timeout: 10 * time.Second}
	resp, err := client.Do(reqOut)
	if err != nil {
		writeJSON(w, http.StatusBadGateway, map[string]string{"error": "Сервис геокодирования недоступен"})
		return
	}
	defer resp.Body.Close()

	body, err := io.ReadAll(io.LimitReader(resp.Body, 1<<20))
	if err != nil || resp.StatusCode != http.StatusOK {
		writeJSON(w, http.StatusBadGateway, map[string]string{"error": "Не удалось получить координаты"})
		return
	}

	var arr []struct {
		Lat         string `json:"lat"`
		Lon         string `json:"lon"`
		DisplayName string `json:"display_name"`
	}
	if err := json.Unmarshal(body, &arr); err != nil || len(arr) == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "Адрес не найден. Уточните город и улицу или укажите точку на карте."})
		return
	}
	lat, err1 := strconv.ParseFloat(arr[0].Lat, 64)
	lon, err2 := strconv.ParseFloat(arr[0].Lon, 64)
	if err1 != nil || err2 != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": "Некорректный ответ геокодера"})
		return
	}

	writeJSON(w, http.StatusOK, map[string]any{
		"lat":   lat,
		"lon":   lon,
		"label": arr[0].DisplayName,
	})
}
