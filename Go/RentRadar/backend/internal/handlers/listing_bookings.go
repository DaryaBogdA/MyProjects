package handlers

import (
	"database/sql"
	"encoding/json"
	"net/http"
	"strconv"
	"strings"
	"time"
)

type bookingRow struct {
	ID           int       `json:"id"`
	ListingID    int       `json:"listing_id"`
	ListingTitle string    `json:"listing_title"`
	GuestID      int       `json:"guest_id,omitempty"`
	GuestName    string    `json:"guest_name,omitempty"`
	CheckIn      time.Time `json:"check_in"`
	CheckOut     time.Time `json:"check_out"`
	Status       string    `json:"status"`
	CreatedAt    time.Time `json:"created_at"`
}

func (h *ListingHandler) CreateListingBooking(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	listingID, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || listingID <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Некорректное объявление."})
		return
	}

	var req struct {
		CheckIn  string `json:"check_in"`
		CheckOut string `json:"check_out"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Некорректные данные запроса."})
		return
	}

	checkIn, err := time.Parse("2006-01-02", strings.TrimSpace(req.CheckIn))
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Укажите корректную дату заезда."})
		return
	}
	checkOut, err := time.Parse("2006-01-02", strings.TrimSpace(req.CheckOut))
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Укажите корректную дату выезда."})
		return
	}
	if !checkOut.After(checkIn) {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Дата выезда должна быть позже даты заезда."})
		return
	}

	today := time.Now().Truncate(24 * time.Hour)
	if checkIn.Before(today) {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Дата заезда не может быть в прошлом."})
		return
	}

	var ownerID int
	var listingType string
	err = h.DB.QueryRow(
		`SELECT user_id, listing_type
		 FROM listings
		 WHERE id = ? AND is_active = 1 AND moderation_status = 'approved'`,
		listingID,
	).Scan(&ownerID, &listingType)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "Объявление не найдено."})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if listingType != "rent" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "Бронирование доступно только для аренды."})
		return
	}
	if ownerID == userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "Нельзя забронировать своё объявление."})
		return
	}

	var overlapCount int
	err = h.DB.QueryRow(
		`SELECT COUNT(*)
		 FROM bookings
		 WHERE listing_id = ?
		   AND status IN ('pending', 'approved')
		   AND NOT (check_out <= ? OR check_in >= ?)`,
		listingID, checkIn, checkOut,
	).Scan(&overlapCount)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if overlapCount > 0 {
		writeJSON(w, http.StatusConflict, map[string]string{"error": "На эти даты уже есть бронирование. Выберите другие даты."})
		return
	}

	res, err := h.DB.Exec(
		`INSERT INTO bookings (listing_id, user_id, check_in, check_out, status)
		 VALUES (?, ?, ?, ?, 'pending')`,
		listingID, userID, checkIn, checkOut,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	id, _ := res.LastInsertId()
	writeJSON(w, http.StatusCreated, map[string]any{
		"message":    "booking created",
		"booking_id": id,
		"status":     "pending",
	})
}

func (h *ListingHandler) GetMyBookings(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	rows, err := h.DB.Query(
		`SELECT b.id, b.listing_id, l.title, b.check_in, b.check_out, b.status, b.created_at
		 FROM bookings b
		 JOIN listings l ON l.id = b.listing_id
		 WHERE b.user_id = ?
		 ORDER BY b.id DESC`,
		userID,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]bookingRow, 0)
	for rows.Next() {
		var row bookingRow
		if err := rows.Scan(&row.ID, &row.ListingID, &row.ListingTitle, &row.CheckIn, &row.CheckOut, &row.Status, &row.CreatedAt); err != nil {
			continue
		}
		out = append(out, row)
	}

	writeJSON(w, http.StatusOK, out)
}

func (h *ListingHandler) GetIncomingBookings(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	rows, err := h.DB.Query(
		`SELECT b.id, b.listing_id, l.title, b.user_id,
		        COALESCE(NULLIF(TRIM(u.first_name), ''), u.email, u.phone, 'Пользователь'),
		        b.check_in, b.check_out, b.status, b.created_at
		 FROM bookings b
		 JOIN listings l ON l.id = b.listing_id
		 JOIN users u ON u.id = b.user_id
		 WHERE l.user_id = ?
		 ORDER BY b.id DESC`,
		userID,
	)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]bookingRow, 0)
	for rows.Next() {
		var row bookingRow
		if err := rows.Scan(&row.ID, &row.ListingID, &row.ListingTitle, &row.GuestID, &row.GuestName, &row.CheckIn, &row.CheckOut, &row.Status, &row.CreatedAt); err != nil {
			continue
		}
		out = append(out, row)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *ListingHandler) UpdateBookingStatus(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid booking id"})
		return
	}

	var req struct {
		Status string `json:"status"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}
	req.Status = strings.ToLower(strings.TrimSpace(req.Status))
	if req.Status != "approved" && req.Status != "rejected" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "status must be approved or rejected"})
		return
	}

	var listingID int
	var ownerID int
	var curStatus string
	var checkIn, checkOut time.Time
	err = h.DB.QueryRow(
		`SELECT b.listing_id, l.user_id, b.status, b.check_in, b.check_out
		 FROM bookings b
		 JOIN listings l ON l.id = b.listing_id
		 WHERE b.id = ?`,
		id,
	).Scan(&listingID, &ownerID, &curStatus, &checkIn, &checkOut)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "booking not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if ownerID != userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "only listing owner can update booking"})
		return
	}
	if strings.ToLower(curStatus) != "pending" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "only pending booking can be updated"})
		return
	}

	if req.Status == "approved" {
		var overlap int
		err = h.DB.QueryRow(
			`SELECT COUNT(*)
			 FROM bookings
			 WHERE listing_id = ?
			   AND id <> ?
			   AND status = 'approved'
			   AND NOT (check_out <= ? OR check_in >= ?)`,
			listingID, id, checkIn, checkOut,
		).Scan(&overlap)
		if err != nil {
			writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
			return
		}
		if overlap > 0 {
			writeJSON(w, http.StatusConflict, map[string]string{"error": "dates overlap with approved booking"})
			return
		}
	}

	_, err = h.DB.Exec(`UPDATE bookings SET status = ? WHERE id = ?`, req.Status, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "booking updated", "status": req.Status})
}

func (h *ListingHandler) DeleteBooking(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodDelete {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid booking id"})
		return
	}
	var bookUserID int
	var status string
	err = h.DB.QueryRow(`SELECT user_id, status FROM bookings WHERE id = ?`, id).Scan(&bookUserID, &status)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "booking not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if bookUserID != userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "only your booking can be cancelled"})
		return
	}
	st := strings.ToLower(strings.TrimSpace(status))
	if st != "pending" && st != "approved" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "this booking cannot be cancelled"})
		return
	}
	if _, err := h.DB.Exec(`DELETE FROM bookings WHERE id = ?`, id); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "booking cancelled"})
}
