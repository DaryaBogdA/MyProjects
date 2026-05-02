package app

import (
	"database/sql"
	"net/http"
	"path/filepath"

	"rentradar/backend/internal/handlers"
	"rentradar/backend/internal/middleware"
)

func NewRouter(db *sql.DB, projectRoot string) *http.ServeMux {
	mux := http.NewServeMux()

	authHandler := handlers.NewAuthHandler(db)
	listingHandler := handlers.NewListingHandler(db)

	mux.HandleFunc("GET /api/health", handlers.HealthCheck)
	mux.HandleFunc("POST /api/register", authHandler.Register)
	mux.HandleFunc("POST /api/login", authHandler.Login)
	mux.HandleFunc("GET /api/listings", listingHandler.GetListings)
	mux.HandleFunc("GET /api/listings/{id}", listingHandler.GetListing)

	mux.HandleFunc("GET /api/me", middleware.AuthMiddleware(db, authHandler.GetMe))
	mux.HandleFunc("POST /api/listings", middleware.AuthMiddleware(db, listingHandler.CreateListing))
	mux.HandleFunc("POST /api/logout", authHandler.Logout)

	staticDir := filepath.Join(projectRoot, "web", "static")
	templatesDir := filepath.Join(projectRoot, "web", "templates")

	mux.Handle("GET /static/", http.StripPrefix("/static/", http.FileServer(http.Dir(staticDir))))
	mux.HandleFunc("GET /", serveTemplate(filepath.Join(templatesDir, "index.html")))
	mux.HandleFunc("GET /index.html", serveTemplate(filepath.Join(templatesDir, "index.html")))
	mux.HandleFunc("GET /login.html", serveTemplate(filepath.Join(templatesDir, "login.html")))
	mux.HandleFunc("GET /profile.html", serveTemplate(filepath.Join(templatesDir, "profile.html")))
	mux.HandleFunc("GET /sale.html", serveTemplate(filepath.Join(templatesDir, "sale.html")))
	mux.HandleFunc("GET /reviews.html", serveTemplate(filepath.Join(templatesDir, "reviews.html")))
	mux.HandleFunc("GET /create-listing.html", serveTemplate(filepath.Join(templatesDir, "create-listing.html")))

	mux.HandleFunc("GET /api/favorites", middleware.AuthMiddleware(db, listingHandler.GetFavorites))
	mux.HandleFunc("POST /api/favorites", middleware.AuthMiddleware(db, listingHandler.AddToFavorites))
	mux.HandleFunc("DELETE /api/favorites/{id}", middleware.AuthMiddleware(db, listingHandler.RemoveFromFavorites))
	mux.HandleFunc("GET /api/my-listings", middleware.AuthMiddleware(db, listingHandler.GetMyListings))
	mux.HandleFunc("PUT /api/profile", middleware.AuthMiddleware(db, authHandler.UpdateProfile))
	mux.HandleFunc("POST /api/change-password", middleware.AuthMiddleware(db, authHandler.ChangePassword))
	mux.HandleFunc("DELETE /api/listings/{id}", middleware.AuthMiddleware(db, listingHandler.DeleteListing))
	return mux
}

func serveTemplate(path string) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		http.ServeFile(w, r, path)
	}
}
