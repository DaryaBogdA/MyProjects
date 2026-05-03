package handlers

import (
	"database/sql"
	"encoding/json"
	"errors"
	"net/http"
	"strings"

	"rentradar/backend/internal/models"

	"github.com/go-sql-driver/mysql"
	"golang.org/x/crypto/bcrypt"
)

type AuthHandler struct {
	DB *sql.DB
}

func NewAuthHandler(db *sql.DB) *AuthHandler {
	return &AuthHandler{DB: db}
}

func (h *AuthHandler) Register(w http.ResponseWriter, r *http.Request) {
	var req models.RegisterRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}

	if req.Email == "" && req.Phone == "" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "email or phone required"})
		return
	}
	if req.Password == "" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "password required"})
		return
	}
	if len(req.Password) < 6 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "password must be at least 6 characters"})
		return
	}

	hashedPassword, err := bcrypt.GenerateFromPassword([]byte(req.Password), bcrypt.DefaultCost)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": "failed to hash password"})
		return
	}

	emailTrim := strings.TrimSpace(req.Email)
	phoneTrim := strings.TrimSpace(req.Phone)

	var normalizedEmail string
	var emailArg any
	if emailTrim != "" {
		normalizedEmail = strings.ToLower(emailTrim)
		emailArg = normalizedEmail
	} else {
		emailArg = nil
	}
	var phoneArg any
	if phoneTrim != "" {
		phoneArg = phoneTrim
	} else {
		phoneArg = nil
	}

	query := `INSERT INTO users (email, phone, password_hash, first_name, last_name, role) 
              VALUES (?, ?, ?, ?, ?, 'user')`

	result, err := h.DB.Exec(query, emailArg, phoneArg, string(hashedPassword), strings.TrimSpace(req.FirstName), strings.TrimSpace(req.LastName))
	if err != nil {
		var sqlErr *mysql.MySQLError
		if errors.As(err, &sqlErr) && sqlErr.Number == 1062 {
			writeJSON(w, http.StatusConflict, map[string]string{"error": "user with this email or phone already exists"})
			return
		}
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	userID, _ := result.LastInsertId()

	writeJSON(w, http.StatusCreated, models.AuthResponse{
		UserID:    int(userID),
		Email:     normalizedEmail,
		Phone:     phoneTrim,
		FirstName: strings.TrimSpace(req.FirstName),
		LastName:  strings.TrimSpace(req.LastName),
		Role:      "user",
	})
}

func (h *AuthHandler) Login(w http.ResponseWriter, r *http.Request) {
	var req models.LoginRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request body"})
		return
	}

	if req.Identifier == "" || req.Password == "" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "identifier and password required"})
		return
	}

	identifier := strings.TrimSpace(req.Identifier)
	if strings.Contains(identifier, "@") {
		identifier = strings.ToLower(identifier)
	}

	var user models.User
	var email sql.NullString
	var phone sql.NullString
	var passwordHash sql.NullString
	var firstName sql.NullString
	var lastName sql.NullString
	var role sql.NullString
	query := `SELECT id, email, phone, password_hash, first_name, last_name, role, is_blocked 
              FROM users WHERE email = ? OR phone = ?`

	err := h.DB.QueryRow(query, identifier, identifier).Scan(
		&user.ID, &email, &phone, &passwordHash, &firstName, &lastName, &role, &user.IsBlocked,
	)

	if err != nil {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "invalid credentials"})
		return
	}
	if email.Valid {
		user.Email = email.String
	}
	if phone.Valid {
		user.Phone = phone.String
	}
	if passwordHash.Valid {
		user.PasswordHash = passwordHash.String
	}
	if firstName.Valid {
		user.FirstName = firstName.String
	}
	if lastName.Valid {
		user.LastName = lastName.String
	}
	if role.Valid {
		user.Role = role.String
	}

	if user.IsBlocked {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "account is blocked"})
		return
	}

	if err := bcrypt.CompareHashAndPassword([]byte(user.PasswordHash), []byte(req.Password)); err != nil {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "invalid credentials"})
		return
	}

	writeJSON(w, http.StatusOK, models.AuthResponse{
		UserID:    user.ID,
		Email:     user.Email,
		Phone:     user.Phone,
		FirstName: user.FirstName,
		LastName:  user.LastName,
		Role:      user.Role,
	})
}

func (h *AuthHandler) Logout(w http.ResponseWriter, r *http.Request) {
	writeJSON(w, http.StatusOK, map[string]string{
		"message": "logged out",
	})
}

func (h *AuthHandler) GetMe(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	var user models.User
	var email sql.NullString
	var phone sql.NullString
	var firstName sql.NullString
	var lastName sql.NullString
	var role sql.NullString
	query := `SELECT id, email, phone, first_name, last_name, role, is_blocked 
              FROM users WHERE id = ?`

	err := h.DB.QueryRow(query, userID).Scan(
		&user.ID, &email, &phone, &firstName, &lastName, &role, &user.IsBlocked,
	)

	if err != nil {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "user not found"})
		return
	}
	if email.Valid {
		user.Email = email.String
	}
	if phone.Valid {
		user.Phone = phone.String
	}
	if firstName.Valid {
		user.FirstName = firstName.String
	}
	if lastName.Valid {
		user.LastName = lastName.String
	}
	if role.Valid {
		user.Role = role.String
	}

	writeJSON(w, http.StatusOK, user)
}

func (h *AuthHandler) UpdateProfile(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	var req struct {
		FirstName string `json:"first_name"`
		LastName  string `json:"last_name"`
		Phone     string `json:"phone"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}

	_, err := h.DB.Exec("UPDATE users SET first_name = ?, last_name = ?, phone = ? WHERE id = ?",
		req.FirstName, req.LastName, req.Phone, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "profile updated"})
}

func (h *AuthHandler) ChangePassword(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	var req struct {
		CurrentPassword string `json:"current_password"`
		NewPassword     string `json:"new_password"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}

	var hash string
	h.DB.QueryRow("SELECT password_hash FROM users WHERE id = ?", userID).Scan(&hash)

	if err := bcrypt.CompareHashAndPassword([]byte(hash), []byte(req.CurrentPassword)); err != nil {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "current password is incorrect"})
		return
	}

	newHash, err := bcrypt.GenerateFromPassword([]byte(req.NewPassword), bcrypt.DefaultCost)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": "failed to hash password"})
		return
	}

	_, err = h.DB.Exec("UPDATE users SET password_hash = ? WHERE id = ?", string(newHash), userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "password changed"})
}
