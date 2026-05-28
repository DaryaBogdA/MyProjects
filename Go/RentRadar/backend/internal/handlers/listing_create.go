package handlers

import (
	"encoding/json"
	"errors"
	"fmt"
	"io"
	"mime/multipart"
	"net/http"
	"os"
	"path/filepath"
	"strconv"
	"strings"

	"rentradar/backend/internal/models"
)

func decodeCreateListingRequest(r *http.Request) (models.CreateListingRequest, *multipart.Form, error) {
	var req models.CreateListingRequest

	ct := strings.ToLower(strings.TrimSpace(r.Header.Get("Content-Type")))
	switch {
	case strings.HasPrefix(ct, "application/json"):
		if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
			return req, nil, err
		}
		return req, nil, nil

	case strings.HasPrefix(ct, "multipart/form-data"):
		if err := r.ParseMultipartForm(32 << 20); err != nil {
			return req, nil, err
		}
		form := r.MultipartForm
		if form == nil {
			return req, nil, errors.New("empty multipart form")
		}
		f := form.Value

		req.Title = strings.TrimSpace(firstForm(f, "title"))
		req.Description = strings.TrimSpace(firstForm(f, "description"))
		req.ListingType = strings.TrimSpace(firstForm(f, "listing_type"))
		req.Address = strings.TrimSpace(firstForm(f, "address"))
		req.City = strings.TrimSpace(firstForm(f, "city"))
		req.District = strings.TrimSpace(firstForm(f, "district"))
		req.AvailableFrom = strings.TrimSpace(firstForm(f, "available_from"))
		req.PropertyType = strings.TrimSpace(firstForm(f, "property_type"))
		if pa, err := strconv.ParseFloat(strings.TrimSpace(firstForm(f, "plot_area")), 64); err == nil {
			req.PlotArea = pa
		}

		if p, err := strconv.ParseFloat(strings.TrimSpace(firstForm(f, "price")), 64); err == nil {
			req.Price = p
		}
		if a, err := strconv.ParseFloat(strings.TrimSpace(firstForm(f, "area")), 64); err == nil {
			req.Area = a
		}
		if rooms, err := strconv.Atoi(strings.TrimSpace(firstForm(f, "rooms"))); err == nil {
			req.Rooms = rooms
		}
		if fl, err := strconv.Atoi(strings.TrimSpace(firstForm(f, "floor"))); err == nil {
			req.Floor = fl
		}
		if tf, err := strconv.Atoi(strings.TrimSpace(firstForm(f, "total_floors"))); err == nil {
			req.TotalFloors = tf
		}

		util := strings.TrimSpace(firstForm(f, "utilities_included"))
		req.UtilitiesIncluded = util == "1" || strings.EqualFold(util, "true") || strings.EqualFold(util, "on")

		if latStr := strings.TrimSpace(firstForm(f, "latitude")); latStr != "" {
			if v, err := strconv.ParseFloat(latStr, 64); err == nil {
				req.Latitude = &v
			}
		}
		if lngStr := strings.TrimSpace(firstForm(f, "longitude")); lngStr != "" {
			if v, err := strconv.ParseFloat(lngStr, 64); err == nil {
				req.Longitude = &v
			}
		}

		return req, form, nil

	default:
		return req, nil, errors.New("unsupported content type")
	}
}

func firstForm(v map[string][]string, key string) string {
	if v == nil {
		return ""
	}
	if vals, ok := v[key]; ok && len(vals) > 0 {
		return vals[0]
	}
	return ""
}

func (h *ListingHandler) saveListingPhotos(form *multipart.Form, listingID int64, startIndex int) ([]string, error) {
	if h.uploadsDir == "" || form == nil {
		return nil, nil
	}
	fileHeaders := form.File["photos"]
	if len(fileHeaders) == 0 {
		return nil, nil
	}
	if err := os.MkdirAll(h.uploadsDir, 0750); err != nil {
		return nil, err
	}

	extOK := map[string]bool{
		".jpg": true, ".jpeg": true, ".png": true, ".webp": true, ".gif": true,
	}

	paths := make([]string, 0, len(fileHeaders))
	for i, fh := range fileHeaders {
		ext := strings.ToLower(filepath.Ext(fh.Filename))
		if !extOK[ext] {
			ext = ".jpg"
		}
		name := fmt.Sprintf("%d_%d%s", listingID, startIndex+i, ext)
		destPath := filepath.Join(h.uploadsDir, name)

		src, err := fh.Open()
		if err != nil {
			continue
		}

		dest, err := os.OpenFile(destPath, os.O_WRONLY|os.O_CREATE|os.O_TRUNC, 0644)
		if err != nil {
			src.Close()
			continue
		}
		_, copyErr := io.Copy(dest, src)
		src.Close()
		dest.Close()
		if copyErr != nil {
			continue
		}
		paths = append(paths, "/static/uploads/listings/"+name)
	}

	if len(paths) == 0 {
		return nil, nil
	}
	return paths, nil
}
