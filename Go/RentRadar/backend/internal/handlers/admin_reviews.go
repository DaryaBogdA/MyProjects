package handlers

import (
	"database/sql"
	"encoding/json"
	"net/http"
	"strconv"
	"strings"
)

type adminPendingReviewRow struct {
	ID               int    `json:"id"`
	ListingID        int    `json:"listing_id"`
	ListingTitle     string `json:"listing_title"`
	UserID           int    `json:"user_id"`
	AuthorName       string `json:"author_name"`
	Rating           int    `json:"rating"`
	Comment          string `json:"comment,omitempty"`
	ModerationStatus string `json:"moderation_status"`
}

func (h *ListingHandler) AdminListPendingReviews(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	rows, err := h.DB.Query(
		`SELECT r.id, r.listing_id, l.title, r.user_id,
		 COALESCE(NULLIF(TRIM(u.first_name), ''), u.email, u.phone, 'Пользователь'),
		 r.rating, r.comment, r.moderation_status
		 FROM reviews r
		 JOIN listings l ON l.id = r.listing_id
		 JOIN users u ON u.id = r.user_id
		 WHERE r.moderation_status = 'pending'
		 ORDER BY r.id ASC`,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()
	out := make([]adminPendingReviewRow, 0)
	for rows.Next() {
		var row adminPendingReviewRow
		var c sql.NullString
		if err := rows.Scan(&row.ID, &row.ListingID, &row.ListingTitle, &row.UserID, &row.AuthorName, &row.Rating, &c, &row.ModerationStatus); err != nil {
			continue
		}
		if c.Valid {
			row.Comment = c.String
		}
		out = append(out, row)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *ListingHandler) AdminSetReviewModeration(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPut {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}
	var body struct {
		ModerationStatus string `json:"moderation_status"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid body"})
		return
	}
	st := strings.ToLower(strings.TrimSpace(body.ModerationStatus))
	if st != "approved" && st != "rejected" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "moderation_status must be approved or rejected"})
		return
	}
	res, err := h.DB.Exec(`UPDATE reviews SET moderation_status = ? WHERE id = ?`, st, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "review not found"})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "updated", "moderation_status": st})
}
