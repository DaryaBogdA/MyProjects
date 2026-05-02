package middleware

import (
	"context"
	"database/sql"
	"net/http"
	"strconv"
)

func AuthMiddleware(db *sql.DB, next http.HandlerFunc) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		userIDRaw := r.Header.Get("X-User-ID")
		if userIDRaw == "" {
			userIDRaw = r.URL.Query().Get("user_id")
		}
		if userIDRaw == "" {
			writeJSONError(w, http.StatusUnauthorized, "missing user id")
			return
		}

		userID, err := strconv.Atoi(userIDRaw)
		if err != nil || userID <= 0 {
			writeJSONError(w, http.StatusUnauthorized, "invalid user id")
			return
		}

		var exists int
		err = db.QueryRow("SELECT id FROM users WHERE id = ?", userID).Scan(&exists)
		if err == sql.ErrNoRows {
			writeJSONError(w, http.StatusUnauthorized, "user not found")
			return
		}
		if err != nil {
			writeJSONError(w, http.StatusInternalServerError, "user lookup failed")
			return
		}

		ctx := context.WithValue(r.Context(), "user_id", userID)
		next(w, r.WithContext(ctx))
	}
}

func writeJSONError(w http.ResponseWriter, status int, message string) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	w.Write([]byte(`{"error":"` + message + `"}`))
}
