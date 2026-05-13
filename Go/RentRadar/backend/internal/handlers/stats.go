package handlers

import (
	"database/sql"
	"net/http"
	"strings"
	"time"
)

type statsListingRow struct {
	ID            int        `json:"id"`
	Title         string     `json:"title"`
	ListingType   string     `json:"listing_type"`
	ViewsCount    int        `json:"views_count"`
	AverageRating float64    `json:"average_rating"`
	ReviewsCount  int        `json:"reviews_count"`
	City          string     `json:"city"`
	Photos        string     `json:"photos"`
	Price         float64    `json:"price"`
	Area          float64    `json:"area"`
	Rooms         int        `json:"rooms"`
	Floor         int        `json:"floor"`
	TotalFloors   int        `json:"total_floors"`
	District      string     `json:"district"`
	Description   string     `json:"description"`
	AvailableFrom *time.Time `json:"available_from,omitempty"`
}

func (h *ListingHandler) GetPublicStats(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}

	sort := strings.ToLower(strings.TrimSpace(r.URL.Query().Get("sort")))
	if sort != "views" {
		sort = "rating"
	}

	baseFrom := `
		FROM (
			SELECT l.id, l.title, l.listing_type, l.views_count,
			       COALESCE((SELECT AVG(r.rating) FROM reviews r WHERE r.listing_id = l.id AND COALESCE(r.moderation_status,'approved')='approved'), 0) AS stat_avg,
			       COALESCE((SELECT COUNT(*) FROM reviews r2 WHERE r2.listing_id = l.id AND COALESCE(r2.moderation_status,'approved')='approved'), 0) AS stat_rc,
			       COALESCE(l.city, '') AS stat_city,
			       COALESCE(l.photos, '') AS stat_photos,
			       l.price AS stat_price,
			       l.area AS stat_area,
			       l.rooms AS stat_rooms,
			       l.floor AS stat_floor,
			       l.total_floors AS stat_total_floors,
			       COALESCE(l.district, '') AS stat_district,
			       COALESCE(l.description, '') AS stat_description,
			       l.available_from AS stat_available_from
			FROM listings l
			WHERE l.is_active = 1 AND l.moderation_status = 'approved'
		) u
	`

	var query string
	if sort == "views" {
		query = `
			SELECT u.id, u.title, u.listing_type, u.views_count,
			       u.stat_avg, u.stat_rc, u.stat_city,
			       u.stat_photos, u.stat_price, u.stat_area, u.stat_rooms, u.stat_floor, u.stat_total_floors,
			       u.stat_district, u.stat_description, u.stat_available_from
		` + baseFrom + `
			ORDER BY u.views_count DESC, u.id DESC`
	} else {
		query = `
			SELECT u.id, u.title, u.listing_type, u.views_count,
			       u.stat_avg, u.stat_rc, u.stat_city,
			       u.stat_photos, u.stat_price, u.stat_area, u.stat_rooms, u.stat_floor, u.stat_total_floors,
			       u.stat_district, u.stat_description, u.stat_available_from
		` + baseFrom + `
			ORDER BY (u.stat_rc = 0) ASC,
			         CASE WHEN u.stat_rc > 0 THEN u.stat_avg ELSE 0 END DESC,
			         CASE WHEN u.stat_rc > 0 THEN u.stat_rc ELSE 0 END DESC,
			         CASE WHEN u.stat_rc > 0 THEN u.views_count ELSE 0 END DESC,
			         u.title ASC`
	}

	rows, err := h.DB.Query(query)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]statsListingRow, 0)
	for rows.Next() {
		var row statsListingRow
		var avail sql.NullTime
		if err := rows.Scan(
			&row.ID, &row.Title, &row.ListingType, &row.ViewsCount,
			&row.AverageRating, &row.ReviewsCount, &row.City,
			&row.Photos, &row.Price, &row.Area, &row.Rooms, &row.Floor, &row.TotalFloors,
			&row.District, &row.Description, &avail,
		); err != nil {
			continue
		}
		if avail.Valid {
			t := avail.Time
			row.AvailableFrom = &t
		}
		out = append(out, row)
	}

	writeJSON(w, http.StatusOK, map[string]interface{}{
		"sort":     sort,
		"listings": out,
	})
}
