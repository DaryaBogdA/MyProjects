package handlers

import (
	"database/sql"
	"encoding/json"
	"net/http"
	"strconv"
	"strings"
	"time"

	"rentradar/backend/internal/models"
)

type MessageHandler struct {
	DB *sql.DB
}

func NewMessageHandler(db *sql.DB) *MessageHandler {
	return &MessageHandler{DB: db}
}

func (h *MessageHandler) StartConversation(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	var req struct {
		ListingID int    `json:"listing_id"`
		Message   string `json:"message"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil || req.ListingID <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}

	var ownerID int
	var listingTitle string
	err := h.DB.QueryRow(
		`SELECT user_id, title FROM listings WHERE id = ? AND is_active = 1 AND moderation_status = 'approved'`,
		req.ListingID,
	).Scan(&ownerID, &listingTitle)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if ownerID == userID {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "cannot message yourself"})
		return
	}

	var convID int
	err = h.DB.QueryRow(
		`SELECT id FROM conversations WHERE listing_id = ? AND owner_id = ? AND customer_id = ?`,
		req.ListingID, ownerID, userID,
	).Scan(&convID)
	if err == sql.ErrNoRows {
		res, exErr := h.DB.Exec(
			`INSERT INTO conversations (listing_id, owner_id, customer_id, created_at) VALUES (?, ?, ?, NOW())`,
			req.ListingID, ownerID, userID,
		)
		if exErr != nil {
			writeJSON(w, http.StatusInternalServerError, map[string]string{"error": exErr.Error()})
			return
		}
		lastID, _ := res.LastInsertId()
		convID = int(lastID)
	} else if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	body := strings.TrimSpace(req.Message)
	if body != "" {
		if _, exErr := h.DB.Exec(
			`INSERT INTO messages (conversation_id, sender_id, body, created_at) VALUES (?, ?, ?, NOW())`,
			convID, userID, body,
		); exErr != nil {
			writeJSON(w, http.StatusInternalServerError, map[string]string{"error": exErr.Error()})
			return
		}
	}

	writeJSON(w, http.StatusOK, map[string]any{
		"conversation_id": convID,
		"listing_id":      req.ListingID,
		"listing_title":   listingTitle,
	})
}

func (h *MessageHandler) GetConversations(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	rows, err := h.DB.Query(`
		SELECT
			c.id, c.listing_id, c.owner_id, c.customer_id,
			l.title,
			(SELECT m.body FROM messages m WHERE m.conversation_id = c.id ORDER BY m.id DESC LIMIT 1) as last_body,
			COALESCE((SELECT m.created_at FROM messages m WHERE m.conversation_id = c.id ORDER BY m.id DESC LIMIT 1), c.created_at) as last_at
		FROM conversations c
		JOIN listings l ON l.id = c.listing_id
		WHERE c.owner_id = ? OR c.customer_id = ?
		ORDER BY last_at DESC`, userID, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]models.Conversation, 0)
	for rows.Next() {
		var c models.Conversation
		var lastBody sql.NullString
		var lastAt time.Time
		if err := rows.Scan(&c.ID, &c.ListingID, &c.OwnerID, &c.CustomerID, &c.ListingTitle, &lastBody, &lastAt); err != nil {
			continue
		}
		if lastBody.Valid {
			c.LastMessage = lastBody.String
		}
		c.LastAt = lastAt
		out = append(out, c)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *MessageHandler) GetConversationMessages(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid conversation id"})
		return
	}

	var ownerID, customerID int
	err = h.DB.QueryRow(`SELECT owner_id, customer_id FROM conversations WHERE id = ?`, id).Scan(&ownerID, &customerID)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "conversation not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if userID != ownerID && userID != customerID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "access denied"})
		return
	}

	rows, err := h.DB.Query(`
		SELECT m.id, m.conversation_id, m.sender_id, COALESCE(u.first_name, u.email, u.phone, ''), m.body, m.created_at
		FROM messages m
		LEFT JOIN users u ON u.id = m.sender_id
		WHERE m.conversation_id = ?
		ORDER BY m.id ASC`, id)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	out := make([]models.ChatMessage, 0)
	for rows.Next() {
		var m models.ChatMessage
		var senderName sql.NullString
		if err := rows.Scan(&m.ID, &m.ConversationID, &m.SenderID, &senderName, &m.Body, &m.CreatedAt); err != nil {
			continue
		}
		if senderName.Valid {
			m.SenderName = senderName.String
		}
		out = append(out, m)
	}
	writeJSON(w, http.StatusOK, out)
}

func (h *MessageHandler) SendMessage(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	id, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || id <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid conversation id"})
		return
	}
	var req struct {
		Body string `json:"body"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}
	body := strings.TrimSpace(req.Body)
	if body == "" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "message is empty"})
		return
	}

	var ownerID, customerID int
	err = h.DB.QueryRow(`SELECT owner_id, customer_id FROM conversations WHERE id = ?`, id).Scan(&ownerID, &customerID)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "conversation not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if userID != ownerID && userID != customerID {
		writeJSON(w, http.StatusForbidden, map[string]string{"error": "access denied"})
		return
	}

	res, err := h.DB.Exec(`INSERT INTO messages (conversation_id, sender_id, body, created_at) VALUES (?, ?, ?, NOW())`, id, userID, body)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	msgID, _ := res.LastInsertId()
	writeJSON(w, http.StatusCreated, map[string]any{"id": msgID})
}
