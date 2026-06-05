package handlers

import (
	"bytes"
	"encoding/json"
	"errors"
	"fmt"
	"image"
	"image/jpeg"
	"io"
	"mime/multipart"
	"net/http"
	"os"
	"path/filepath"
	"strconv"
	"strings"

	_ "image/gif"
	_ "image/png"

	"rentradar/backend/internal/models"

	"github.com/nfnt/resize"
)

func convertToJPEG(imgData io.Reader, quality int, maxWidth, maxHeight uint) ([]byte, error) {
	img, _, err := image.Decode(imgData)
	if err != nil {
		return nil, fmt.Errorf("не удалось декодировать изображение: %w", err)
	}

	bounds := img.Bounds()
	width := uint(bounds.Dx())
	height := uint(bounds.Dy())

	if width > maxWidth || height > maxHeight {
		img = resize.Thumbnail(maxWidth, maxHeight, img, resize.Lanczos3)
	}

	var buf bytes.Buffer
	err = jpeg.Encode(&buf, img, &jpeg.Options{Quality: quality})
	if err != nil {
		return nil, fmt.Errorf("ошибка кодирования в JPEG: %w", err)
	}

	return buf.Bytes(), nil
}

func processAndSaveImage(fh *multipart.FileHeader, destPath string) error {
	src, err := fh.Open()
	if err != nil {
		return fmt.Errorf("не удалось открыть файл: %w", err)
	}
	defer src.Close()

	jpegData, err := convertToJPEG(src, 85, 1920, 1920)
	if err != nil {
		return err
	}

	dest, err := os.OpenFile(destPath, os.O_WRONLY|os.O_CREATE|os.O_TRUNC, 0644)
	if err != nil {
		return fmt.Errorf("не удалось создать файл: %w", err)
	}
	defer dest.Close()

	_, err = dest.Write(jpegData)
	if err != nil {
		return fmt.Errorf("ошибка записи файла: %w", err)
	}

	return nil
}

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
		if err := r.ParseMultipartForm(100 << 20); err != nil {
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

	paths := make([]string, 0, len(fileHeaders))
	for i, fh := range fileHeaders {
		name := fmt.Sprintf("%d_%d.jpg", listingID, startIndex+i)
		destPath := filepath.Join(h.uploadsDir, name)

		if err := processAndSaveImage(fh, destPath); err != nil {
			fmt.Printf("Ошибка обработки фото %s: %v\n", fh.Filename, err)
			continue
		}
		paths = append(paths, "/static/uploads/listings/"+name)
	}

	if len(paths) == 0 {
		return nil, nil
	}
	return paths, nil
}
