package handlers

import (
	"database/sql"
	"encoding/json"
	"net/http"
	"strconv"
	"strings"

	"rentradar/backend/internal/models"
)

type ListingHandler struct {
	DB         *sql.DB
	uploadsDir string
	reportsDir string
}

func NewListingHandler(db *sql.DB, uploadsDir, reportsDir string) *ListingHandler {
	return &ListingHandler{DB: db, uploadsDir: uploadsDir, reportsDir: reportsDir}
}

func (h *ListingHandler) GetListings(w http.ResponseWriter, r *http.Request) {
	listingType := r.URL.Query().Get("type")
	city := r.URL.Query().Get("city")
	minPrice := r.URL.Query().Get("minPrice")
	maxPrice := r.URL.Query().Get("maxPrice")
	minArea := r.URL.Query().Get("minArea")
	maxArea := r.URL.Query().Get("maxArea")
	rooms := r.URL.Query().Get("rooms")

	query := `SELECT l.id, l.user_id, l.title, l.description, l.listing_type, l.price, l.AREA, l.rooms, 
          l.FLOOR, l.total_floors, l.address, l.city, l.district, l.available_from, l.deposit, 
          l.utilities_included, l.photos, l.is_active, l.moderation_status, l.views_count,
          l.latitude, l.longitude,
          COALESCE((SELECT AVG(r.rating) FROM reviews r WHERE r.listing_id = l.id AND COALESCE(r.moderation_status, 'approved') = 'approved'), 0) as average_rating,
          COALESCE((SELECT COUNT(*) FROM reviews r2 WHERE r2.listing_id = l.id AND COALESCE(r2.moderation_status, 'approved') = 'approved'), 0) as reviews_count
          FROM listings l WHERE l.is_active = 1 AND l.moderation_status = 'approved'`

	args := []interface{}{}

	if listingType != "" {
		query += " AND LOWER(listing_type) = LOWER(?)"
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
		var lat sql.NullFloat64
		var lng sql.NullFloat64
		var utilities sql.NullBool
		var active sql.NullBool

		err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &utilities, &photos, &active, &l.ModerationStatus, &l.ViewsCount, &lat, &lng, &l.AverageRating, &l.ReviewsCount)

		if err != nil {
			continue
		}

		l.UtilitiesIncluded = utilities.Valid && utilities.Bool
		l.IsActive = active.Valid && active.Bool

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
		if lat.Valid {
			v := lat.Float64
			l.Latitude = &v
		}
		if lng.Valid {
			v := lng.Float64
			l.Longitude = &v
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
	var lat sql.NullFloat64
	var lng sql.NullFloat64
	var utilities sql.NullBool
	var active sql.NullBool
	query := `SELECT id, user_id, title, description, listing_type, price, AREA, rooms, 
              FLOOR, total_floors, address, city, district, available_from, deposit, 
              utilities_included, photos, is_active, moderation_status, views_count,
              latitude, longitude
              FROM listings WHERE id = ?`

	err = h.DB.QueryRow(query, id).Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType,
		&l.Price, &area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
		&availableFrom, &deposit, &utilities, &photos, &active,
		&l.ModerationStatus, &l.ViewsCount, &lat, &lng)

	if err != nil {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}

	l.UtilitiesIncluded = utilities.Valid && utilities.Bool
	l.IsActive = active.Valid && active.Bool

	viewerID := 0
	if hdr := strings.TrimSpace(r.Header.Get("X-User-ID")); hdr != "" {
		viewerID, _ = strconv.Atoi(hdr)
	}
	if !h.canViewListing(&l, viewerID) {
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
	if lat.Valid {
		v := lat.Float64
		l.Latitude = &v
	}
	if lng.Valid {
		v := lng.Float64
		l.Longitude = &v
	}

	if l.IsActive && strings.EqualFold(l.ModerationStatus, "approved") &&
		!(viewerID > 0 && viewerID == l.UserID) {
		h.DB.Exec("UPDATE listings SET views_count = views_count + 1 WHERE id = ?", id)
	}

	writeJSON(w, http.StatusOK, l)
}

func (h *ListingHandler) CreateListing(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	req, multipartForm, err := decodeCreateListingRequest(r)
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}

	if strings.TrimSpace(req.Title) == "" || strings.TrimSpace(req.ListingType) == "" ||
		req.ListingType != "rent" && req.ListingType != "sale" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "title and listing_type (rent/sale) are required"})
		return
	}

	query := `INSERT INTO listings (user_id, title, description, listing_type, price, area, rooms, 
              floor, total_floors, address, city, district, available_from, deposit, utilities_included, photos,
              latitude, longitude, moderation_status) 
              VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, 'pending')`

	var availableDate interface{}
	if req.AvailableFrom != "" {
		availableDate = req.AvailableFrom
	}

	var latArg, lngArg interface{}
	if req.Latitude != nil {
		latArg = *req.Latitude
	}
	if req.Longitude != nil {
		lngArg = *req.Longitude
	}

	result, err := h.DB.Exec(query, userID, req.Title, req.Description, req.ListingType,
		req.Price, req.Area, req.Rooms, req.Floor, req.TotalFloors, req.Address,
		req.City, req.District, availableDate, req.Deposit, req.UtilitiesIncluded, req.Photos,
		latArg, lngArg)

	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	id, _ := result.LastInsertId()

	if multipartForm != nil && len(multipartForm.File["photos"]) > 0 {
		paths, saveErr := h.saveListingPhotos(multipartForm, id, 0)
		if saveErr == nil && len(paths) > 0 {
			_, _ = h.DB.Exec(`UPDATE listings SET photos = ? WHERE id = ?`, strings.Join(paths, ","), id)
		}
	}

	writeJSON(w, http.StatusCreated, map[string]interface{}{
		"message":           "listing submitted for moderation",
		"id":                id,
		"moderation_status": "pending",
	})
}

func mergePhotoURLs(existing string, add []string) string {
	var parts []string
	for _, p := range strings.Split(existing, ",") {
		p = strings.TrimSpace(p)
		if p != "" {
			parts = append(parts, p)
		}
	}
	for _, p := range add {
		p = strings.TrimSpace(p)
		if p != "" {
			parts = append(parts, p)
		}
	}
	return strings.Join(parts, ",")
}

func (h *ListingHandler) UpdateListing(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	var ownerID int
	err = h.DB.QueryRow(`SELECT user_id FROM listings WHERE id = ?`, id).Scan(&ownerID)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if ownerID != userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "access denied"})
		return
	}

	req, multipartForm, err := decodeCreateListingRequest(r)
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}

	if strings.TrimSpace(req.Title) == "" || strings.TrimSpace(req.ListingType) == "" ||
		req.ListingType != "rent" && req.ListingType != "sale" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "title and listing_type (rent/sale) are required"})
		return
	}

	var prevPhotos string
	if err := h.DB.QueryRow(`SELECT COALESCE(photos, '') FROM listings WHERE id = ?`, id).Scan(&prevPhotos); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	fileCount := 0
	for _, p := range strings.Split(prevPhotos, ",") {
		if strings.TrimSpace(p) != "" {
			fileCount++
		}
	}

	var addPaths []string
	if multipartForm != nil && len(multipartForm.File["photos"]) > 0 {
		var saveErr error
		addPaths, saveErr = h.saveListingPhotos(multipartForm, int64(id), fileCount)
		if saveErr != nil {
			writeJSON(w, http.StatusInternalServerError, map[string]string{"error": saveErr.Error()})
			return
		}
	}

	finalPhotos := prevPhotos
	if len(addPaths) > 0 {
		finalPhotos = mergePhotoURLs(prevPhotos, addPaths)
	}
	if strings.TrimSpace(req.Photos) != "" && len(addPaths) == 0 {
		finalPhotos = strings.TrimSpace(req.Photos)
	}

	var availableDate interface{}
	if req.AvailableFrom != "" {
		availableDate = req.AvailableFrom
	}

	var latArg, lngArg interface{}
	if req.Latitude != nil {
		latArg = *req.Latitude
	}
	if req.Longitude != nil {
		lngArg = *req.Longitude
	}

	_, err = h.DB.Exec(`UPDATE listings SET title=?, description=?, listing_type=?, price=?, area=?, rooms=?, 
		floor=?, total_floors=?, address=?, city=?, district=?, available_from=?, deposit=?, utilities_included=?, photos=?,
		latitude=?, longitude=?, moderation_status='pending'
		WHERE id=? AND user_id=?`,
		req.Title, req.Description, req.ListingType, req.Price, req.Area, req.Rooms,
		req.Floor, req.TotalFloors, req.Address, req.City, req.District, availableDate, req.Deposit, req.UtilitiesIncluded, finalPhotos,
		latArg, lngArg, id, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]interface{}{
		"message":           "listing updated",
		"moderation_status": "pending",
		"id":                id,
	})
}

type adminListingBody struct {
	models.CreateListingRequest
	ModerationStatus string `json:"moderation_status"`
}

func (h *ListingHandler) AdminListListings(w http.ResponseWriter, r *http.Request) {
	status := strings.ToLower(strings.TrimSpace(r.URL.Query().Get("status")))
	includeInactive := r.URL.Query().Get("include_inactive") == "1" ||
		strings.EqualFold(r.URL.Query().Get("include_inactive"), "true")

	q := `SELECT id, user_id, title, description, listing_type, price, AREA, rooms, 
              FLOOR, total_floors, address, city, district, available_from, deposit, 
              utilities_included, photos, is_active, moderation_status, views_count
              FROM listings WHERE 1=1`
	args := []interface{}{}
	if !includeInactive {
		q += ` AND is_active = 1`
	}
	if status != "" && status != "all" {
		q += ` AND moderation_status = ?`
		args = append(args, status)
	}
	q += ` ORDER BY id DESC LIMIT 500`

	rows, err := h.DB.Query(q, args...)
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
		var utilities sql.NullBool
		var active sql.NullBool

		if err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &utilities, &photos, &active, &l.ModerationStatus,
			&l.ViewsCount); err != nil {
			continue
		}

		l.UtilitiesIncluded = utilities.Valid && utilities.Bool
		l.IsActive = active.Valid && active.Bool
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

func (h *ListingHandler) AdminUpdateListing(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	var body adminListingBody
	if err := json.NewDecoder(r.Body).Decode(&body); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}

	if strings.TrimSpace(body.Title) == "" || strings.TrimSpace(body.ListingType) == "" ||
		body.ListingType != "rent" && body.ListingType != "sale" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "title and listing_type (rent/sale) are required"})
		return
	}

	var curMod string
	if err := h.DB.QueryRow(`SELECT COALESCE(moderation_status, 'pending') FROM listings WHERE id = ?`, id).Scan(&curMod); err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	} else if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	modStatus := strings.ToLower(strings.TrimSpace(body.ModerationStatus))
	if modStatus != "" && modStatus != "pending" && modStatus != "approved" && modStatus != "rejected" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid moderation_status"})
		return
	}
	if modStatus == "" {
		modStatus = curMod
	}

	var availableDate interface{}
	if body.AvailableFrom != "" {
		availableDate = body.AvailableFrom
	}

	photos := strings.TrimSpace(body.Photos)
	if photos == "" {
		var prev sql.NullString
		_ = h.DB.QueryRow(`SELECT photos FROM listings WHERE id = ?`, id).Scan(&prev)
		if prev.Valid {
			photos = prev.String
		}
	}

	_, err = h.DB.Exec(`UPDATE listings SET title=?, description=?, listing_type=?, price=?, area=?, rooms=?, 
		floor=?, total_floors=?, address=?, city=?, district=?, available_from=?, deposit=?, utilities_included=?, photos=?, moderation_status=?
		WHERE id=?`,
		body.Title, body.Description, body.ListingType, body.Price, body.Area, body.Rooms,
		body.Floor, body.TotalFloors, body.Address, body.City, body.District, availableDate,
		body.Deposit, body.UtilitiesIncluded, photos, modStatus, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]interface{}{
		"message":           "listing updated",
		"moderation_status": modStatus,
		"id":                id,
	})
}

func (h *ListingHandler) AdminDeleteListing(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	res, err := h.DB.Exec(`UPDATE listings SET is_active = 0 WHERE id = ?`, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "listing removed from catalog"})
}

func (h *ListingHandler) GetFavorites(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	query := `SELECT l.id, l.user_id, l.title, l.description, l.listing_type, l.price, l.AREA, l.rooms, 
              l.FLOOR, l.total_floors, l.address, l.city, l.district, l.available_from, l.deposit, 
              l.utilities_included, l.photos, l.is_active, l.moderation_status, l.views_count
              FROM listings l 
              INNER JOIN favorites f ON l.id = f.listing_id 
              WHERE f.user_id = ? AND l.is_active = 1 AND l.moderation_status = 'approved'`

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
		var utilities sql.NullBool
		var active sql.NullBool

		err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &utilities, &photos, &active,
			&l.ModerationStatus, &l.ViewsCount)
		if err != nil {
			continue
		}

		l.UtilitiesIncluded = utilities.Valid && utilities.Bool
		l.IsActive = active.Valid && active.Bool

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

	var mod string
	var active bool
	if err := h.DB.QueryRow(
		`SELECT moderation_status, is_active FROM listings WHERE id = ?`,
		req.ListingID).Scan(&mod, &active); err != nil {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if !active || strings.ToLower(mod) != "approved" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "listing is not available for favorites"})
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
              utilities_included, photos, is_active, moderation_status, views_count 
              FROM listings WHERE user_id = ? ORDER BY id DESC`

	rows, err := h.DB.Query(query, userID)
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
		var utilities sql.NullBool
		var active sql.NullBool

		if err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &utilities, &photos, &active, &l.ModerationStatus, &l.ViewsCount); err != nil {
			continue
		}

		l.UtilitiesIncluded = utilities.Valid && utilities.Bool
		l.IsActive = active.Valid && active.Bool

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
	if err := h.DB.QueryRow("SELECT user_id FROM listings WHERE id = ?", id).Scan(&ownerID); err != nil {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if ownerID != userID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "access denied"})
		return
	}

	tx, err := h.DB.Begin()
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer tx.Rollback()

	_, _ = tx.Exec("DELETE FROM favorites WHERE listing_id = ?", id)
	_, _ = tx.Exec("DELETE FROM reviews WHERE listing_id = ?", id)
	_, _ = tx.Exec("DELETE FROM bookings WHERE listing_id = ?", id)
	_, _ = tx.Exec("DELETE FROM user_reports WHERE listing_id = ?", id)
	_, _ = tx.Exec("DELETE m FROM messages m JOIN conversations c ON c.id = m.conversation_id WHERE c.listing_id = ?", id)
	_, _ = tx.Exec("DELETE FROM conversations WHERE listing_id = ?", id)
	res, err := tx.Exec("DELETE FROM listings WHERE id = ? AND user_id = ?", id, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if err := tx.Commit(); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "listing deleted permanently"})
}

func (h *ListingHandler) userHasRole(userID int, want string) bool {
	if userID <= 0 {
		return false
	}
	var role sql.NullString
	if err := h.DB.QueryRow(`SELECT COALESCE(role, 'user') FROM users WHERE id = ?`, userID).Scan(&role); err != nil {
		return false
	}
	if !role.Valid {
		return false
	}
	return strings.EqualFold(strings.TrimSpace(role.String), want)
}

func (h *ListingHandler) canViewListing(l *models.Listing, viewerID int) bool {
	if viewerID > 0 && l.UserID == viewerID {
		return true
	}
	if viewerID > 0 && h.userHasRole(viewerID, "admin") {
		return true
	}
	return l.IsActive && strings.EqualFold(l.ModerationStatus, "approved")
}

func (h *ListingHandler) AdminListPending(w http.ResponseWriter, r *http.Request) {
	query := `SELECT id, user_id, title, description, listing_type, price, AREA, rooms, 
              FLOOR, total_floors, address, city, district, available_from, deposit, 
              utilities_included, photos, is_active, moderation_status, views_count
              FROM listings WHERE is_active = 1 AND moderation_status = 'pending' ORDER BY id ASC`

	rows, err := h.DB.Query(query)
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
		var utilities sql.NullBool
		var active sql.NullBool

		if err := rows.Scan(&l.ID, &l.UserID, &l.Title, &description, &l.ListingType, &l.Price,
			&area, &roomCount, &floor, &totalFloors, &l.Address, &l.City, &district,
			&availableFrom, &deposit, &utilities, &photos, &active, &l.ModerationStatus,
			&l.ViewsCount); err != nil {
			continue
		}

		l.UtilitiesIncluded = utilities.Valid && utilities.Bool
		l.IsActive = active.Valid && active.Bool
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

func (h *ListingHandler) AdminApproveListing(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}
	res, err := h.DB.Exec(`UPDATE listings SET moderation_status = 'approved' WHERE id = ? AND is_active = 1 AND moderation_status = 'pending'`, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "pending listing not found"})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "listing approved"})
}

func (h *ListingHandler) AdminRejectListing(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}
	res, err := h.DB.Exec(`UPDATE listings SET moderation_status = 'rejected' WHERE id = ? AND is_active = 1 AND moderation_status = 'pending'`, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "pending listing not found"})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "listing rejected"})
}
