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
	staticRoot := filepath.Join(projectRoot, "web", "static")
	listingsUploadDir := filepath.Join(staticRoot, "uploads", "listings")
	adminReportsDir := filepath.Join(projectRoot, "data", "admin_reports")
	listingHandler := handlers.NewListingHandler(db, listingsUploadDir, adminReportsDir)
	messageHandler := handlers.NewMessageHandler(db)
	reportHandler := handlers.NewReportHandler(db)

	mux.HandleFunc("POST /api/register", authHandler.Register)
	mux.HandleFunc("POST /api/login", authHandler.Login)
	mux.HandleFunc("GET /api/listings", listingHandler.GetListings)
	mux.HandleFunc("GET /api/geocode", middleware.AuthMiddleware(db, listingHandler.GeocodeQuery))
	mux.HandleFunc("GET /api/listings/{id}/reviews", listingHandler.GetListingReviews)
	mux.HandleFunc("POST /api/listings/{id}/reviews", middleware.AuthMiddleware(db, listingHandler.CreateListingReview))
	mux.HandleFunc("POST /api/listings/{id}/bookings", middleware.AuthMiddleware(db, listingHandler.CreateListingBooking))
	mux.HandleFunc("GET /api/listings/{id}", listingHandler.GetListing)

	mux.HandleFunc("GET /api/me", middleware.AuthMiddleware(db, authHandler.GetMe))
	mux.HandleFunc("POST /api/listings", middleware.AuthMiddleware(db, listingHandler.CreateListing))
	mux.HandleFunc("PUT /api/listings/{id}", middleware.AuthMiddleware(db, listingHandler.UpdateListing))

	mux.HandleFunc("GET /api/admin/pending-listings", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminListPending)))
	mux.HandleFunc("GET /api/admin/listings", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminListListings)))
	mux.HandleFunc("PUT /api/admin/listings/{id}", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminUpdateListing)))
	mux.HandleFunc("DELETE /api/admin/listings/{id}", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminDeleteListing)))
	mux.HandleFunc("POST /api/admin/listings/{id}/approve", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminApproveListing)))
	mux.HandleFunc("POST /api/admin/listings/{id}/reject", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminRejectListing)))

	mux.HandleFunc("GET /api/admin/users", middleware.AuthMiddleware(db, middleware.AdminOnly(authHandler.AdminListUsers)))
	mux.HandleFunc("GET /api/admin/users/{id}", middleware.AuthMiddleware(db, middleware.AdminOnly(authHandler.AdminGetUser)))
	mux.HandleFunc("POST /api/admin/users/{id}/block", middleware.AuthMiddleware(db, middleware.AdminOnly(authHandler.AdminBlockUser)))
	mux.HandleFunc("POST /api/admin/users/{id}/unblock", middleware.AuthMiddleware(db, middleware.AdminOnly(authHandler.AdminUnblockUser)))
	mux.HandleFunc("POST /api/logout", authHandler.Logout)
	mux.HandleFunc("POST /api/messages/start", middleware.AuthMiddleware(db, messageHandler.StartConversation))
	mux.HandleFunc("GET /api/messages/conversations", middleware.AuthMiddleware(db, messageHandler.GetConversations))
	mux.HandleFunc("GET /api/messages/{id}", middleware.AuthMiddleware(db, messageHandler.GetMessages))
	mux.HandleFunc("POST /api/messages/{id}", middleware.AuthMiddleware(db, messageHandler.SendMessage))

	mux.HandleFunc("GET /api/reviews/by-me", middleware.AuthMiddleware(db, listingHandler.GetReviewsWrittenByMe))
	mux.HandleFunc("GET /api/reviews/about-my-listings", middleware.AuthMiddleware(db, listingHandler.GetReviewsAboutMyListings))
	mux.HandleFunc("GET /api/bookings/my", middleware.AuthMiddleware(db, listingHandler.GetMyBookings))
	mux.HandleFunc("GET /api/bookings/incoming", middleware.AuthMiddleware(db, listingHandler.GetIncomingBookings))
	mux.HandleFunc("PUT /api/bookings/{id}/status", middleware.AuthMiddleware(db, listingHandler.UpdateBookingStatus))

	mux.HandleFunc("POST /api/reports", middleware.AuthMiddleware(db, reportHandler.CreateUserReport))
	mux.HandleFunc("GET /api/admin/reports", middleware.AuthMiddleware(db, middleware.AdminOnly(reportHandler.AdminListUserReports)))
	mux.HandleFunc("PUT /api/admin/reports/{id}", middleware.AuthMiddleware(db, middleware.AdminOnly(reportHandler.AdminUpdateUserReport)))

	mux.HandleFunc("GET /api/admin/reviews/pending", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminListPendingReviews)))
	mux.HandleFunc("PUT /api/admin/reviews/{id}/moderation", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminSetReviewModeration)))

	mux.HandleFunc("POST /api/admin/saved-reports/snapshot", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminCreateSnapshotReport)))
	mux.HandleFunc("POST /api/admin/saved-reports", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminCreateSavedReport)))
	mux.HandleFunc("GET /api/admin/saved-reports/{id}", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminGetSavedReport)))
	mux.HandleFunc("GET /api/admin/saved-reports", middleware.AuthMiddleware(db, middleware.AdminOnly(listingHandler.AdminListSavedReports)))

	templatesDir := filepath.Join(projectRoot, "web", "templates")

	mux.Handle("GET /static/", http.StripPrefix("/static/", http.FileServer(http.Dir(staticRoot))))
	mux.HandleFunc("GET /", serveTemplate(filepath.Join(templatesDir, "index.html")))
	mux.HandleFunc("GET /index.html", serveTemplate(filepath.Join(templatesDir, "index.html")))
	mux.HandleFunc("GET /login.html", serveTemplate(filepath.Join(templatesDir, "login.html")))
	mux.HandleFunc("GET /profile.html", serveTemplate(filepath.Join(templatesDir, "profile.html")))
	mux.HandleFunc("GET /sale.html", serveTemplate(filepath.Join(templatesDir, "sale.html")))
	mux.HandleFunc("GET /reviews.html", serveTemplate(filepath.Join(templatesDir, "reviews.html")))
	mux.HandleFunc("GET /create-listing.html", serveTemplate(filepath.Join(templatesDir, "create-listing.html")))
	mux.HandleFunc("GET /admin.html", serveTemplate(filepath.Join(templatesDir, "admin.html")))
	mux.HandleFunc("GET /listing-details.html", serveTemplate(filepath.Join(templatesDir, "listing-details.html")))
	mux.HandleFunc("GET /messages.html", serveTemplate(filepath.Join(templatesDir, "messages.html")))
	mux.HandleFunc("GET /bookings.html", serveTemplate(filepath.Join(templatesDir, "bookings.html")))
	mux.HandleFunc("GET /news.html", serveTemplate(filepath.Join(templatesDir, "news.html")))
	mux.HandleFunc("GET /about.html", serveTemplate(filepath.Join(templatesDir, "about.html")))
	mux.HandleFunc("GET /privacy.html", serveTemplate(filepath.Join(templatesDir, "privacy.html")))
	mux.HandleFunc("GET /terms.html", serveTemplate(filepath.Join(templatesDir, "terms.html")))
	mux.HandleFunc("GET /help-sell.html", serveTemplate(filepath.Join(templatesDir, "help-sell.html")))
	mux.HandleFunc("GET /help-buy.html", serveTemplate(filepath.Join(templatesDir, "help-buy.html")))
	mux.HandleFunc("GET /safety.html", serveTemplate(filepath.Join(templatesDir, "safety.html")))
	mux.HandleFunc("GET /support.html", serveTemplate(filepath.Join(templatesDir, "support.html")))

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
