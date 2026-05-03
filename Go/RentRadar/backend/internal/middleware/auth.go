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

		var role sql.NullString
		var blocked bool
		err = db.QueryRow(
			`SELECT COALESCE(NULLIF(TRIM(role), ''), 'user'), COALESCE(is_blocked, 0) FROM users WHERE id = ?`,
			userID,
		).Scan(&role, &blocked)
		if err == sql.ErrNoRows {
			writeJSONError(w, http.StatusUnauthorized, "user not found")
			return
		}
		if err != nil {
			writeJSONError(w, http.StatusInternalServerError, "user lookup failed")
			return
		}
		if blocked {
			writeJSONError(w, http.StatusForbidden, "account is blocked")
			return
		}

		roleStr := "user"
		if role.Valid {
			roleStr = role.String
		}

		ctx := context.WithValue(r.Context(), "user_id", userID)
		ctx = context.WithValue(ctx, "user_role", roleStr)
		next(w, r.WithContext(ctx))
	}
}

func AdminOnly(next http.HandlerFunc) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		role, ok := r.Context().Value("user_role").(string)
		if !ok || role != "admin" {
			writeJSONError(w, http.StatusForbidden, "admin access required")
			return
		}
		next(w, r)
	}
}

func writeJSONError(w http.ResponseWriter, status int, message string) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	w.Write([]byte(`{"error":"` + message + `"}`))
}
