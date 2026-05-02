package handlers

import (
	"encoding/json"
	"net/http"
	"time"

	"rentradar/backend/internal/models"
)

func CreateBooking(w http.ResponseWriter, r *http.Request) {
	var req models.BookingCreateRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{
			"error": "invalid json body",
		})
		return
	}

	booking := models.Booking{
		ID:        1,
		ListingID: req.ListingID,
		UserID:    req.UserID,
		CheckIn:   mustParseDate(req.CheckIn),
		CheckOut:  mustParseDate(req.CheckOut),
		Status:    "pending",
	}

	writeJSON(w, http.StatusCreated, map[string]any{
		"message": "Booking request received",
		"booking": booking,
	})
}

func mustParseDate(raw string) time.Time {
	parsed, err := time.Parse("2006-01-02", raw)
	if err != nil {
		return time.Time{}
	}
	return parsed
}
