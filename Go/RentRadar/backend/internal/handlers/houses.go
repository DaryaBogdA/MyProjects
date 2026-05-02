package handlers

import (
	"net/http"

	"rentradar/backend/internal/models"
)

func GetHouses(w http.ResponseWriter, _ *http.Request) {
	listings := []models.Listing{
		{
			ID:          1,
			UserID:      1,
			Title:       "Cozy mountain house",
			ListingType: "rent",
			Price:       120,
			Address:     "Sample address",
			City:        "Bansko",
			IsActive:    true,
		},
	}

	writeJSON(w, http.StatusOK, listings)
}
