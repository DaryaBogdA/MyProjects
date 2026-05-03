package handlers

import (
	"database/sql"
	"encoding/json"
	"errors"
	"fmt"
	"net/http"
	"os"
	"path/filepath"
	"strconv"
	"strings"
	"time"
)

type ReportHandler struct {
	DB *sql.DB
}

func NewReportHandler(db *sql.DB) *ReportHandler {
	return &ReportHandler{DB: db}
}

type userReportRow struct {
	ID             int       `json:"id"`
	ReporterID     int       `json:"reporter_id"`
	ReporterName   string    `json:"reporter_name,omitempty"`
	ReportedUserID int       `json:"reported_user_id"`
	ReportedName   string    `json:"reported_user_name,omitempty"`
	ListingID      *int      `json:"listing_id,omitempty"`
	Reason         string    `json:"reason"`
	Status         string    `json:"status"`
	CreatedAt      time.Time `json:"created_at"`
}

func (h *ReportHandler) CreateUserReport(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	reporterID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	var body struct {
		ReportedUserID int    `json:"reported_user_id"`
		ListingID      *int   `json:"listing_id"`
		Reason         string `json:"reason"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}
	if body.ReportedUserID <= 0 || body.ReportedUserID == reporterID {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid reported user"})
		return
	}
	reason := strings.TrimSpace(body.Reason)
	if reason == "" || len(reason) > 4000 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "reason required"})
		return
	}
	var reportedID int
	if err := h.DB.QueryRow(`SELECT id FROM users WHERE id = ? LIMIT 1`, body.ReportedUserID).Scan(&reportedID); err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "user not found"})
		return
	} else if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	var lid interface{}
	if body.ListingID != nil && *body.ListingID > 0 {
		lid = *body.ListingID
	} else {
		lid = nil
	}
	res, err := h.DB.Exec(
		`INSERT INTO user_reports (reporter_id, reported_user_id, listing_id, reason, status) VALUES (?, ?, ?, ?, 'pending')`,
		reporterID, body.ReportedUserID, lid, reason,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	id, _ := res.LastInsertId()
	writeJSON(w, http.StatusCreated, map[string]any{"message": "report submitted", "id": id})
}

func (h *ReportHandler) AdminListUserReports(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	status := strings.ToLower(strings.TrimSpace(r.URL.Query().Get("status")))
	q := `SELECT ur.id, ur.reporter_id,
		COALESCE(NULLIF(TRIM(ur1.first_name), ''), ur1.email, ur1.phone, ''),
		ur.reported_user_id,
		COALESCE(NULLIF(TRIM(ur2.first_name), ''), ur2.email, ur2.phone, ''),
		ur.listing_id, ur.reason, ur.status, ur.created_at
		FROM user_reports ur
		JOIN users ur1 ON ur1.id = ur.reporter_id
		JOIN users ur2 ON ur2.id = ur.reported_user_id`
	args := []interface{}{}
	if status == "pending" || status == "resolved" || status == "dismissed" {
		q += ` WHERE ur.status = ?`
		args = append(args, status)
	}
	q += ` ORDER BY ur.id DESC`
	rows, err := h.DB.Query(q, args...)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()
	out := make([]userReportRow, 0)
	for rows.Next() {
		var row userReportRow
		var lid sql.NullInt64
		if err := rows.Scan(&row.ID, &row.ReporterID, &row.ReporterName, &row.ReportedUserID, &row.ReportedName, &lid, &row.Reason, &row.Status, &row.CreatedAt); err != nil {
			continue
		}
		if lid.Valid {
			v := int(lid.Int64)
			row.ListingID = &v
		}
		out = append(out, row)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *ReportHandler) AdminUpdateUserReport(w http.ResponseWriter, r *http.Request) {
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
		Status string `json:"status"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid body"})
		return
	}
	st := strings.ToLower(strings.TrimSpace(body.Status))
	if st != "resolved" && st != "dismissed" && st != "pending" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid status"})
		return
	}
	res, err := h.DB.Exec(`UPDATE user_reports SET status = ? WHERE id = ?`, st, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "report not found"})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "updated", "status": st})
}

type savedAdminReportListItem struct {
	ID        int       `json:"id"`
	Title     string    `json:"title"`
	CreatedAt time.Time `json:"created_at"`
	File      string    `json:"file,omitempty"`
}

func adminReportFilename(id int64) string {
	return fmt.Sprintf("report_%d.json", id)
}

func (h *ListingHandler) writeAdminReportDisk(id int64, content string) error {
	if h.reportsDir == "" {
		return errors.New("reports directory not configured")
	}
	if err := os.MkdirAll(h.reportsDir, 0750); err != nil {
		return err
	}
	path := filepath.Join(h.reportsDir, adminReportFilename(id))
	return os.WriteFile(path, []byte(content), 0644)
}

func (h *ListingHandler) readAdminReportFromDisk(rel string) ([]byte, error) {
	if h.reportsDir == "" || strings.TrimSpace(rel) == "" {
		return nil, os.ErrNotExist
	}
	base := filepath.Base(strings.TrimSpace(rel))
	if base == "" || base == "." || base == ".." {
		return nil, os.ErrNotExist
	}
	return os.ReadFile(filepath.Join(h.reportsDir, base))
}

func (h *ListingHandler) insertAdminReportSaveFile(adminID int, title, fileContent string) (int64, error) {
	res, err := h.DB.Exec(
		`INSERT INTO admin_saved_reports (admin_id, title, body, body_file) VALUES (?, ?, '', NULL)`,
		adminID, title,
	)
	if err != nil {
		return 0, err
	}
	id, _ := res.LastInsertId()
	if err := h.writeAdminReportDisk(id, fileContent); err != nil {
		_, _ = h.DB.Exec(`DELETE FROM admin_saved_reports WHERE id = ?`, id)
		return 0, err
	}
	fn := adminReportFilename(id)
	if _, err := h.DB.Exec(`UPDATE admin_saved_reports SET body_file = ? WHERE id = ? AND admin_id = ?`, fn, id, adminID); err != nil {
		_ = os.Remove(filepath.Join(h.reportsDir, fn))
		_, _ = h.DB.Exec(`DELETE FROM admin_saved_reports WHERE id = ?`, id)
		return 0, err
	}
	return id, nil
}

func (h *ListingHandler) AdminCreateSavedReport(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	adminID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	var body struct {
		Title string `json:"title"`
		Body  string `json:"body"`
	}
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid body"})
		return
	}
	title := strings.TrimSpace(body.Title)
	txt := strings.TrimSpace(body.Body)
	if title == "" || txt == "" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "title and body required"})
		return
	}
	newID, err := h.insertAdminReportSaveFile(adminID, title, txt)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	writeJSON(w, http.StatusCreated, map[string]any{
		"id":      newID,
		"message": "saved",
		"file":    adminReportFilename(newID),
	})
}

func (h *ListingHandler) AdminGetSavedReport(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	adminID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}
	var title string
	var body string
	var bodyFile sql.NullString
	err = h.DB.QueryRow(
		`SELECT title, body, body_file FROM admin_saved_reports WHERE id = ? AND admin_id = ?`,
		id, adminID,
	).Scan(&title, &body, &bodyFile)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if bodyFile.Valid && strings.TrimSpace(bodyFile.String) != "" {
		if b, rerr := h.readAdminReportFromDisk(bodyFile.String); rerr == nil && len(b) > 0 {
			body = string(b)
		}
	}
	writeJSON(w, http.StatusOK, map[string]string{"title": title, "body": body})
}

func (h *ListingHandler) AdminListSavedReports(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodGet {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	adminID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	rows, err := h.DB.Query(
		`SELECT id, title, created_at, body_file FROM admin_saved_reports WHERE admin_id = ? ORDER BY id DESC`,
		adminID,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()
	out := make([]savedAdminReportListItem, 0)
	for rows.Next() {
		var row savedAdminReportListItem
		var bf sql.NullString
		if err := rows.Scan(&row.ID, &row.Title, &row.CreatedAt, &bf); err != nil {
			continue
		}
		if bf.Valid && strings.TrimSpace(bf.String) != "" {
			row.File = filepath.Base(strings.TrimSpace(bf.String))
		}
		out = append(out, row)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *ListingHandler) AdminCreateSnapshotReport(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	adminID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	genAt := time.Now().UTC().Format(time.RFC3339)

	urows, err := h.DB.Query(`SELECT id, email, phone, first_name, last_name, role, is_blocked FROM users ORDER BY id`)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	users := make([]adminUserSummary, 0)
	for urows.Next() {
		var u adminUserSummary
		var email, phone, fn, ln, role sql.NullString
		if err := urows.Scan(&u.ID, &email, &phone, &fn, &ln, &role, &u.IsBlocked); err != nil {
			continue
		}
		if email.Valid {
			u.Email = email.String
		}
		if phone.Valid {
			u.Phone = phone.String
		}
		if fn.Valid {
			u.FirstName = fn.String
		}
		if ln.Valid {
			u.LastName = ln.String
		}
		if role.Valid {
			u.Role = role.String
		}
		users = append(users, u)
	}
	urows.Close()
	if err := urows.Err(); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	revRows, err := h.DB.Query(
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
	pendingReviews := make([]adminPendingReviewRow, 0)
	for revRows.Next() {
		var row adminPendingReviewRow
		var c sql.NullString
		if err := revRows.Scan(&row.ID, &row.ListingID, &row.ListingTitle, &row.UserID, &row.AuthorName, &row.Rating, &c, &row.ModerationStatus); err != nil {
			continue
		}
		if c.Valid {
			row.Comment = c.String
		}
		pendingReviews = append(pendingReviews, row)
	}
	revRows.Close()
	if err := revRows.Err(); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	repQ := `SELECT ur.id, ur.reporter_id,
		COALESCE(NULLIF(TRIM(ur1.first_name), ''), ur1.email, ur1.phone, ''),
		ur.reported_user_id,
		COALESCE(NULLIF(TRIM(ur2.first_name), ''), ur2.email, ur2.phone, ''),
		ur.listing_id, ur.reason, ur.status, ur.created_at
		FROM user_reports ur
		JOIN users ur1 ON ur1.id = ur.reporter_id
		JOIN users ur2 ON ur2.id = ur.reported_user_id
		ORDER BY ur.id DESC`
	repRows, err := h.DB.Query(repQ)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	allReports := make([]userReportRow, 0)
	for repRows.Next() {
		var row userReportRow
		var lid sql.NullInt64
		if err := repRows.Scan(&row.ID, &row.ReporterID, &row.ReporterName, &row.ReportedUserID, &row.ReportedName, &lid, &row.Reason, &row.Status, &row.CreatedAt); err != nil {
			continue
		}
		if lid.Valid {
			v := int(lid.Int64)
			row.ListingID = &v
		}
		allReports = append(allReports, row)
	}
	repRows.Close()
	if err := repRows.Err(); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	listModRows, err := h.DB.Query(`SELECT moderation_status, COUNT(*) FROM listings GROUP BY moderation_status`)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	listingModerationCounts := make(map[string]int64)
	for listModRows.Next() {
		var st sql.NullString
		var cnt int64
		if err := listModRows.Scan(&st, &cnt); err != nil {
			continue
		}
		key := "unknown"
		if st.Valid && strings.TrimSpace(st.String) != "" {
			key = st.String
		}
		listingModerationCounts[key] = cnt
	}
	listModRows.Close()

	revModRows, err := h.DB.Query(
		`SELECT COALESCE(r.moderation_status, 'approved'), COUNT(*) FROM reviews r GROUP BY COALESCE(r.moderation_status, 'approved')`,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	reviewModerationCounts := make(map[string]int64)
	for revModRows.Next() {
		var st string
		var cnt int64
		if err := revModRows.Scan(&st, &cnt); err != nil {
			continue
		}
		reviewModerationCounts[st] = cnt
	}
	revModRows.Close()

	pendListRows, err := h.DB.Query(
		`SELECT l.id, l.user_id, l.title, l.city, l.listing_type
		 FROM listings l
		 WHERE l.moderation_status = 'pending'
		 ORDER BY l.id ASC`,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	type pendingListSnap struct {
		ID          int    `json:"id"`
		UserID      int    `json:"user_id"`
		Title       string `json:"title"`
		City        string `json:"city,omitempty"`
		ListingType string `json:"listing_type,omitempty"`
	}
	pendingListings := make([]pendingListSnap, 0)
	for pendListRows.Next() {
		var row pendingListSnap
		var city sql.NullString
		if err := pendListRows.Scan(&row.ID, &row.UserID, &row.Title, &city, &row.ListingType); err != nil {
			continue
		}
		if city.Valid {
			row.City = city.String
		}
		pendingListings = append(pendingListings, row)
	}
	pendListRows.Close()

	var reportStatusCounts struct {
		Pending   int64 `json:"pending"`
		Resolved  int64 `json:"resolved"`
		Dismissed int64 `json:"dismissed"`
	}
	_ = h.DB.QueryRow(`SELECT COUNT(*) FROM user_reports WHERE status = 'pending'`).Scan(&reportStatusCounts.Pending)
	_ = h.DB.QueryRow(`SELECT COUNT(*) FROM user_reports WHERE status = 'resolved'`).Scan(&reportStatusCounts.Resolved)
	_ = h.DB.QueryRow(`SELECT COUNT(*) FROM user_reports WHERE status = 'dismissed'`).Scan(&reportStatusCounts.Dismissed)

	snapshot := map[string]any{
		"generated_at":               genAt,
		"users":                      users,
		"users_total":                len(users),
		"reviews_pending_queue":      pendingReviews,
		"reviews_moderation_counts":  reviewModerationCounts,
		"listings_moderation_counts": listingModerationCounts,
		"listings_pending_queue":     pendingListings,
		"user_reports_all":           allReports,
		"user_reports_status_counts": reportStatusCounts,
	}

	raw, err := json.MarshalIndent(snapshot, "", "  ")
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	title := "Полный снимок панели — " + time.Now().Format("02.01.2006 15:04")
	fileContent := string(raw)

	newID, err := h.insertAdminReportSaveFile(adminID, title, fileContent)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	writeJSON(w, http.StatusCreated, map[string]any{
		"id":      newID,
		"message": "snapshot saved",
		"title":   title,
		"file":    adminReportFilename(newID),
	})
}
