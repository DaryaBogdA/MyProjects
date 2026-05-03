package handlers

import (
	"database/sql"
	"net/http"
	"strconv"

	"rentradar/backend/internal/models"
)

type adminUserSummary struct {
	ID        int    `json:"id"`
	Email     string `json:"email,omitempty"`
	Phone     string `json:"phone,omitempty"`
	FirstName string `json:"first_name,omitempty"`
	LastName  string `json:"last_name,omitempty"`
	Role      string `json:"role,omitempty"`
	IsBlocked bool   `json:"is_blocked"`
}

func (h *AuthHandler) AdminListUsers(w http.ResponseWriter, r *http.Request) {
	rows, err := h.DB.Query(`SELECT id, email, phone, first_name, last_name, role, is_blocked 
		FROM users ORDER BY id DESC`)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]adminUserSummary, 0)
	for rows.Next() {
		var u adminUserSummary
		var email, phone, fn, ln, role sql.NullString
		if err := rows.Scan(&u.ID, &email, &phone, &fn, &ln, &role, &u.IsBlocked); err != nil {
			continue
		}
		if email.Valid {
			u.Email = email.String
		}
		if phone.Valid {
			u.Phone = phone.String
		}
		if fn.Valid {
			u.FirstName = fn.String
		}
		if ln.Valid {
			u.LastName = ln.String
		}
		if role.Valid {
			u.Role = role.String
		}
		out = append(out, u)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *AuthHandler) AdminGetUser(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	id, err := strconv.Atoi(idStr)
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	var u models.User
	var email, phone, fn, ln, role sql.NullString
	err = h.DB.QueryRow(`SELECT id, email, phone, first_name, last_name, role, is_blocked FROM users WHERE id = ?`, id).Scan(
		&u.ID, &email, &phone, &fn, &ln, &role, &u.IsBlocked,
	)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "user not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if email.Valid {
		u.Email = email.String
	}
	if phone.Valid {
		u.Phone = phone.String
	}
	if fn.Valid {
		u.FirstName = fn.String
	}
	if ln.Valid {
		u.LastName = ln.String
	}
	if role.Valid {
		u.Role = role.String
	}
	writeJSON(w, http.StatusOK, u)
}

func (h *AuthHandler) AdminBlockUser(w http.ResponseWriter, r *http.Request) {
	adminID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	idStr := r.PathValue("id")
	targetID, err := strconv.Atoi(idStr)
	if err != nil || targetID <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}
	if targetID == adminID {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "cannot block yourself"})
		return
	}

	var role sql.NullString
	if err := h.DB.QueryRow(`SELECT COALESCE(role, 'user') FROM users WHERE id = ?`, targetID).Scan(&role); err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "user not found"})
		return
	} else if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if role.Valid && role.String == "admin" {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "cannot block an admin"})
		return
	}

	if _, err := h.DB.Exec(`UPDATE users SET is_blocked = 1 WHERE id = ?`, targetID); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "user blocked"})
}

func (h *AuthHandler) AdminUnblockUser(w http.ResponseWriter, r *http.Request) {
	idStr := r.PathValue("id")
	targetID, err := strconv.Atoi(idStr)
	if err != nil || targetID <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid id"})
		return
	}

	res, err := h.DB.Exec(`UPDATE users SET is_blocked = 0 WHERE id = ?`, targetID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	n, _ := res.RowsAffected()
	if n == 0 {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "user not found"})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "user unblocked"})
}
