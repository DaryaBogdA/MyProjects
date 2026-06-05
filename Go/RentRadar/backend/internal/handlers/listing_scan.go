package handlers

import (
	"database/sql"
	"strings"

	"rentradar/backend/internal/models"
)

func normalizeListingType(t string) string {
	return strings.ToLower(strings.TrimSpace(t))
}

func validateListingProperty(req *models.CreateListingRequest) string {
	pt := normalizeListingType(req.PropertyType)
	if pt == "house" && req.PlotArea <= 0 {
		return "plot_area required for house"
	}
	return ""
}

func applyListingNullableFields(l *models.Listing, description sql.NullString, area sql.NullFloat64,
	roomCount, floor, totalFloors sql.NullInt64, district sql.NullString,
	propertyType sql.NullString, plotArea sql.NullFloat64,
	availableFrom sql.NullTime, deposit sql.NullString, photos sql.NullString) {
	if description.Valid {
		l.Description = description.String
	}
	if area.Valid {
		l.Area = area.Float64
	}
	if roomCount.Valid {
		l.Rooms = int(roomCount.Int64)
	}
	if floor.Valid {
		l.Floor = int(floor.Int64)
	}
	if totalFloors.Valid {
		l.TotalFloors = int(totalFloors.Int64)
	}
	if district.Valid {
		l.District = district.String
	}
	if propertyType.Valid {
		l.PropertyType = propertyType.String
	}
	if plotArea.Valid && plotArea.Float64 > 0 {
		l.PlotArea = plotArea.Float64
	}
	if availableFrom.Valid {
		l.AvailableFrom = &availableFrom.Time
	}
	if deposit.Valid {
		l.Deposit = deposit.String
	}
	if photos.Valid {
		l.Photos = photos.String
	}
}
