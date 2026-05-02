package handlers

import (
	"database/sql"
	"encoding/json"
	"net/http"
	"strconv"

	"rentradar/backend/internal/models"
)

type ListingHandler struct {
	DB *sql.DB
}

func NewListingHandler(db *sql.DB) *ListingHandler {
	return &ListingHandler{DB: db}
}

func (h *ListingHandler) GetListings(w http.ResponseWriter, r *http.Request) {
	listingType := r.URL.Query().Get("type")
	city := r.URL.Query().Get("city")
	minPrice := r.URL.Query().Get("minPrice")
	maxPrice := r.URL.Query().Get("maxPrice")
	minArea := r.URL.Query().Get("minArea")
	maxArea := r.URL.Query().Get("maxArea")
	rooms := r.URL.Query().Get("rooms")

	query := `SELECT id, user_id, title, description, listing_type, price, AREA, rooms, 
          FLOOR, total_floors, address, city, district, available_from, deposit, 
          utilities_included, photos, is_active, views_count
          FROM listings WHERE is_active = 1`

	args := []interface{}{}

	if listingType != "" {
		query += " AND listing_type = ?"
		args = append(args, listingType)
	}

	if city != "" {
		query += " AND (city LIKE ? OR district LIKE ? OR address LIKE ? OR title LIKE ?)"
		like := "%" + city + "%"
		args = append(args, like, like, like, like)
	}

	if minPrice != "" {
		query += " AND price >= ?"
		args = append(args, minPrice)
	}

	if maxPrice != "" {
		query += " AND price <= ?"
		args = append(args, maxPrice)
	}

	if minArea != "" {
		query += " AND area >= ?"
		args = append(args, minArea)
	}

	if maxArea != "" {
		query += " AND area <= ?"
		args = append(args, maxArea)
	}

	if rooms == "5" || rooms == "5+" {
		query += " AND rooms >= 5"
	} else if rooms != "" && rooms != "all" {
		query += " AND rooms = ?"
		args = append(args, rooms)
	}

	query += " ORDER BY id DESC"

	rows, err := h.DB.Query(query, args...)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	listings := make([]models.Listing, 0)
	for rows.Next() {
		var l models.Listing
		var description sql.NullString
		var area sql.NullFloat64
		var roomCount sql.NullInt64
		var floor sql.NullInt64
		var totalFloors sql.NullInt64
		var district sql.NullString
		var availableFrom sql.NullTime
		var deposit sql.NullString
		var photos sql.NullString

		err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &l.UtilitiesIncluded, &photos, &l.IsActive,
			&l.ViewsCount)

		if err != nil {
			continue
		}

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

		if availableFrom.Valid {
			l.AvailableFrom = &availableFrom.Time
		} else {
			l.AvailableFrom = nil
		}

		if deposit.Valid {
			l.Deposit = deposit.String
		}
		if photos.Valid {
			l.Photos = photos.String
		}
		listings = append(listings, l)
	}

	writeJSON(w, http.StatusOK, listings)
}

func (h *ListingHandler) GetListing(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	var l models.Listing
	var description sql.NullString
	var area sql.NullFloat64
	var roomCount sql.NullInt64
	var floor sql.NullInt64
	var totalFloors sql.NullInt64
	var district sql.NullString
	var availableFrom sql.NullTime
	var deposit sql.NullString
	var photos sql.NullString
	query := `SELECT id, user_id, title, description, listing_type, price, AREA, rooms, 
              FLOOR, total_floors, address, city, district, available_from, deposit, 
              utilities_included, photos, is_active, views_count
              FROM listings WHERE id = ?`

	err = h.DB.QueryRow(query, id).Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType,
		&l.Price, &area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
		&availableFrom, &deposit, &l.UtilitiesIncluded, &photos, &l.IsActive,
		&l.ViewsCount)

	if err != nil {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}

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

	if availableFrom.Valid {
		l.AvailableFrom = &availableFrom.Time
	} else {
		l.AvailableFrom = nil
	}

	if deposit.Valid {
		l.Deposit = deposit.String
	}
	if photos.Valid {
		l.Photos = photos.String
	}
	h.DB.Exec("UPDATE listings SET views_count = views_count + 1 WHERE id = ?", id)

	writeJSON(w, http.StatusOK, l)
}

func (h *ListingHandler) CreateListing(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	var req models.CreateListingRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}

	query := `INSERT INTO listings (user_id, title, description, listing_type, price, area, rooms, 
              floor, total_floors, address, city, district, available_from, deposit, utilities_included, photos) 
              VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`

	var availableDate interface{}
	if req.AvailableFrom != "" {
		availableDate = req.AvailableFrom
	}

	result, err := h.DB.Exec(query, userID, req.Title, req.Description, req.ListingType,
		req.Price, req.Area, req.Rooms, req.Floor, req.TotalFloors, req.Address,
		req.City, req.District, availableDate, req.Deposit, req.UtilitiesIncluded, req.Photos)

	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	id, _ := result.LastInsertId()
	writeJSON(w, http.StatusCreated, map[string]interface{}{
		"message": "listing created successfully",
		"id":      id,
	})
}

func (h *ListingHandler) GetFavorites(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	query := `SELECT l.id, l.user_id, l.title, l.description, l.listing_type, l.price, l.AREA, l.rooms, 
              l.FLOOR, l.total_floors, l.address, l.city, l.district, l.available_from, l.deposit, 
              l.utilities_included, l.photos, l.is_active, l.views_count
              FROM listings l 
              INNER JOIN favorites f ON l.id = f.listing_id 
              WHERE f.user_id = ? AND l.is_active = 1`

	rows, err := h.DB.Query(query, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	var listings []models.Listing
	for rows.Next() {
		var l models.Listing
		var description sql.NullString
		var area sql.NullFloat64
		var roomCount sql.NullInt64
		var floor sql.NullInt64
		var totalFloors sql.NullInt64
		var district sql.NullString
		var availableFrom sql.NullTime
		var deposit sql.NullString
		var photos sql.NullString

		err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &l.UtilitiesIncluded, &photos, &l.IsActive, &l.ViewsCount)
		if err != nil {
			continue
		}

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
		if availableFrom.Valid {
			l.AvailableFrom = &availableFrom.Time
		}
		if deposit.Valid {
			l.Deposit = deposit.String
		}
		if photos.Valid {
			l.Photos = photos.String
		}
		listings = append(listings, l)
	}

	writeJSON(w, http.StatusOK, listings)
}

func (h *ListingHandler) AddToFavorites(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	var req struct {
		ListingID int `json:"listing_id"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}

	_, err := h.DB.Exec("INSERT IGNORE INTO favorites (user_id, listing_id) VALUES (?, ?)", userID, req.ListingID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "added to favorites"})
}

func (h *ListingHandler) RemoveFromFavorites(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	idStr := r.PathValue("id")
	id, _ := strconv.Atoi(idStr)

	_, err := h.DB.Exec("DELETE FROM favorites WHERE user_id = ? AND listing_id = ?", userID, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "removed from favorites"})
}

func (h *ListingHandler) GetMyListings(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	query := `SELECT id, user_id, title, description, listing_type, price, AREA, rooms, 
              FLOOR, total_floors, address, city, district, available_from, deposit, 
              utilities_included, photos, is_active, views_count 
              FROM listings WHERE user_id = ? ORDER BY id DESC`

	rows, err := h.DB.Query(query, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	var listings []models.Listing
	for rows.Next() {
		var l models.Listing
		var description sql.NullString
		var area sql.NullFloat64
		var roomCount sql.NullInt64
		var floor sql.NullInt64
		var totalFloors sql.NullInt64
		var district sql.NullString
		var availableFrom sql.NullTime
		var deposit sql.NullString
		var photos sql.NullString

		if err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &l.UtilitiesIncluded, &photos, &l.IsActive, &l.ViewsCount); err != nil {
			continue
		}

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
		if availableFrom.Valid {
			l.AvailableFrom = &availableFrom.Time
		}
		if deposit.Valid {
			l.Deposit = deposit.String
		}
		if photos.Valid {
			l.Photos = photos.String
		}
		listings = append(listings, l)
	}

	writeJSON(w, http.StatusOK, listings)
}

func (h *ListingHandler) DeleteListing(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	idStr := r.PathValue("id")
	id, _ := strconv.Atoi(idStr)

	var ownerID int
	h.DB.QueryRow("SELECT user_id FROM listings WHERE id = ?", id).Scan(&ownerID)
	if ownerID != userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "access denied"})
		return
	}

	_, err := h.DB.Exec("UPDATE listings SET is_active = 0 WHERE id = ?", id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "listing deleted"})
}
