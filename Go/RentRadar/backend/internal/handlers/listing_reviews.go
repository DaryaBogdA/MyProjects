package handlers

import (
	"database/sql"
	"encoding/json"
	"errors"
	"math"
	"net/http"
	"strconv"
	"strings"
	"time"

	"rentradar/backend/internal/models"

	"github.com/go-sql-driver/mysql"
)

type listingReviewsResponse struct {
	AverageRating     float64         `json:"average_rating"`
	Count             int             `json:"count"`
	ViewerHasReviewed bool            `json:"viewer_has_reviewed"`
	Items             []models.Review `json:"items"`
}

type listingReviewMineRow struct {
	ID               int       `json:"id"`
	ListingID        int       `json:"listing_id"`
	Rating           int       `json:"rating"`
	Comment          string    `json:"comment,omitempty"`
	CreatedAt        time.Time `json:"created_at"`
	ModerationStatus string    `json:"moderation_status"`
	Title            string    `json:"listing_title"`
	Photos           string    `json:"listing_photos,omitempty"`
	Price            float64   `json:"listing_price"`
	ListingType      string    `json:"listing_type"`
}

type listingReviewAboutRow struct {
	ID               int       `json:"id"`
	ListingID        int       `json:"listing_id"`
	Rating           int       `json:"rating"`
	Comment          string    `json:"comment,omitempty"`
	CreatedAt        time.Time `json:"created_at"`
	ModerationStatus string    `json:"moderation_status"`
	Title            string    `json:"listing_title"`
	Photos           string    `json:"listing_photos,omitempty"`
	Price            float64   `json:"listing_price"`
	ListingType      string    `json:"listing_type"`
	ReviewerName     string    `json:"reviewer_name"`
}

func (h *ListingHandler) GetListingReviews(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}

	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	var l models.Listing
	err = h.DB.QueryRow(
		`SELECT id, user_id, is_active, COALESCE(NULLIF(TRIM(moderation_status), ''), 'pending')
		 FROM listings WHERE id = ?`,
		id,
	).Scan(&l.ID, &l.UserID, &l.IsActive, &l.ModerationStatus)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	viewerID := 0
	if hdr := strings.TrimSpace(r.Header.Get("X-User-ID")); hdr != "" {
		viewerID, _ = strconv.Atoi(hdr)
	}
	if !h.canViewListing(&l, viewerID) {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}

	var avg sql.NullFloat64
	var count int
	_ = h.DB.QueryRow(
		`SELECT COALESCE(AVG(rating), 0), COUNT(*) FROM reviews WHERE listing_id = ? AND COALESCE(moderation_status,'approved') = 'approved'`,
		id,
	).Scan(&avg, &count)

	resp := listingReviewsResponse{
		Count:             count,
		ViewerHasReviewed: false,
		Items:             make([]models.Review, 0),
	}
	if avg.Valid && count > 0 {
		resp.AverageRating = math.Round(avg.Float64*10) / 10
	}

	if viewerID > 0 {
		var dummy int
		err := h.DB.QueryRow(
			`SELECT 1 FROM reviews WHERE listing_id = ? AND user_id = ? LIMIT 1`,
			id, viewerID,
		).Scan(&dummy)
		resp.ViewerHasReviewed = err == nil
	}

	rows, err := h.DB.Query(
		`SELECT r.id, r.listing_id, r.user_id, r.rating, r.comment, r.created_at,
		 COALESCE(NULLIF(TRIM(u.first_name), ''), u.email, u.phone, 'Пользователь')
		 FROM reviews r
		 JOIN users u ON u.id = r.user_id
		 WHERE r.listing_id = ? AND COALESCE(r.moderation_status,'approved') = 'approved'
		 ORDER BY r.id DESC`,
		id,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	for rows.Next() {
		var rev models.Review
		var comment sql.NullString
		if err := rows.Scan(&rev.ID, &rev.ListingID, &rev.UserID, &rev.Rating, &comment, &rev.CreatedAt, &rev.AuthorName); err != nil {
			continue
		}
		if comment.Valid {
			rev.Comment = comment.String
		}
		resp.Items = append(resp.Items, rev)
	}

	writeJSON(w, http.StatusOK, resp)
}

func (h *ListingHandler) CreateListingReview(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}

	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	idStr := r.PathValue("id")
	listingID, err := strconv.Atoi(idStr)
	if err != nil || listingID <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	var ownerID int
	err = h.DB.QueryRow(
		`SELECT user_id FROM listings WHERE id = ? AND is_active = 1 AND moderation_status = 'approved'`,
		listingID,
	).Scan(&ownerID)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if ownerID == userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "нельзя оставить отзыв на своё объявление"})
		return
	}

	var body struct {
		Rating  int    `json:"rating"`
		Comment string `json:"comment"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}
	if body.Rating < 1 || body.Rating > 5 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "оценка от 1 до 5"})
		return
	}
	comment := strings.TrimSpace(body.Comment)

	res, err := h.DB.Exec(
		`INSERT INTO reviews (listing_id, user_id, rating, comment, moderation_status) VALUES (?, ?, ?, ?, 'pending')`,
		listingID, userID, body.Rating, comment,
	)
	if err != nil {
		var sqlErr *mysql.MySQLError
		if errors.As(err, &sqlErr) && sqlErr.Number == 1062 {
			writeJSON(w, http.StatusConflict, map[string]string{"error": "вы уже оставили отзыв к этому объявлению"})
			return
		}
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	newID, _ := res.LastInsertId()
	writeJSON(w, http.StatusCreated, map[string]any{
		"message":           "review submitted for moderation",
		"id":                newID,
		"moderation_status": "pending",
	})
}

func (h *ListingHandler) GetReviewsWrittenByMe(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	rows, err := h.DB.Query(
		`SELECT r.id, r.listing_id, r.rating, r.comment, r.created_at, COALESCE(r.moderation_status,'approved'),
		 l.title, COALESCE(l.photos,''), l.price, l.listing_type
		 FROM reviews r
		 JOIN listings l ON l.id = r.listing_id
		 WHERE r.user_id = ?
		 ORDER BY r.id DESC`,
		userID,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]listingReviewMineRow, 0)
	for rows.Next() {
		var row listingReviewMineRow
		var photos sql.NullString
		var comment sql.NullString
		if err := rows.Scan(&row.ID, &row.ListingID, &row.Rating, &comment, &row.CreatedAt, &row.ModerationStatus, &row.Title, &photos, &row.Price, &row.ListingType); err != nil {
			continue
		}
		if comment.Valid {
			row.Comment = comment.String
		}
		if photos.Valid {
			row.Photos = photos.String
		}
		out = append(out, row)
	}

	writeJSON(w, http.StatusOK, out)
}

func (h *ListingHandler) GetReviewsAboutMyListings(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	rows, err := h.DB.Query(
		`SELECT r.id, r.listing_id, r.rating, r.comment, r.created_at, COALESCE(r.moderation_status,'approved'),
		 l.title, COALESCE(l.photos,''), l.price, l.listing_type,
		 COALESCE(NULLIF(TRIM(u.first_name), ''), u.email, u.phone, 'Пользователь')
		 FROM reviews r
		 JOIN listings l ON l.id = r.listing_id AND l.user_id = ?
		 JOIN users u ON u.id = r.user_id
		 ORDER BY r.id DESC`,
		userID,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]listingReviewAboutRow, 0)
	for rows.Next() {
		var row listingReviewAboutRow
		var photos sql.NullString
		var comment sql.NullString
		if err := rows.Scan(&row.ID, &row.ListingID, &row.Rating, &comment, &row.CreatedAt, &row.ModerationStatus, &row.Title, &photos, &row.Price, &row.ListingType, &row.ReviewerName); err != nil {
			continue
		}
		if comment.Valid {
			row.Comment = comment.String
		}
		if photos.Valid {
			row.Photos = photos.String
		}
		out = append(out, row)
	}

	writeJSON(w, http.StatusOK, out)
}
