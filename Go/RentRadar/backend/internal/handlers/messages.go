package handlers

import (
	"database/sql"
	"encoding/json"
	"net/http"
	"strconv"
	"strings"
	"time"
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
		GuestID   int    `json:"guest_id"`
		Message   string `json:"message"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}

	var ownerID int
	err := h.DB.QueryRow("SELECT user_id FROM listings WHERE id = ? AND is_active = 1", req.ListingID).Scan(&ownerID)
	if err == sql.ErrNoRows {
		writeJSON(w, http.StatusNotFound, map[string]string{"error": "listing not found"})
		return
	}
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	if ownerID == userID {
		if req.GuestID <= 0 || req.GuestID == ownerID {
			writeJSON(w, http.StatusBadRequest, map[string]string{"error": "cannot chat with yourself"})
			return
		}
		var hasBooking int
		err = h.DB.QueryRow(
			`SELECT 1 FROM bookings b WHERE b.listing_id = ? AND b.user_id = ? LIMIT 1`,
			req.ListingID, req.GuestID,
		).Scan(&hasBooking)
		if err == sql.ErrNoRows {
			writeJSON(w, http.StatusForbidden, map[string]string{"error": "no booking with this guest for this listing"})
			return
		}
		if err != nil {
			writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
			return
		}
	}

	customerID := userID
	if ownerID == userID {
		customerID = req.GuestID
	}

	var conversationID int
	err = h.DB.QueryRow(`
		SELECT id FROM conversations 
		WHERE listing_id = ? AND owner_id = ? AND customer_id = ?
	`, req.ListingID, ownerID, customerID).Scan(&conversationID)

	if err == sql.ErrNoRows {
		res, err := h.DB.Exec(`
			INSERT INTO conversations (listing_id, owner_id, customer_id, created_at)
			VALUES (?, ?, ?, NOW())
		`, req.ListingID, ownerID, customerID)
		if err != nil {
			writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
			return
		}
		conversationID64, _ := res.LastInsertId()
		conversationID = int(conversationID64)
	}
	if err != nil && err != sql.ErrNoRows {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	if strings.TrimSpace(req.Message) != "" {
		_, err = h.DB.Exec(`
			INSERT INTO messages (conversation_id, sender_id, body, created_at)
			VALUES (?, ?, ?, NOW())
		`, conversationID, userID, req.Message)
		if err != nil {
			_ = err
		}
	}

	writeJSON(w, http.StatusOK, map[string]interface{}{
		"conversation_id": conversationID,
		"message":         "conversation started",
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
			c.id, 
			c.listing_id, 
			l.title as listing_title,
			COALESCE(
				(SELECT body FROM messages WHERE conversation_id = c.id ORDER BY id DESC LIMIT 1),
				''
			) as last_message,
			COALESCE(
				(SELECT created_at FROM messages WHERE conversation_id = c.id ORDER BY id DESC LIMIT 1),
				c.created_at
			) as last_at
		FROM conversations c
		JOIN listings l ON l.id = c.listing_id
		WHERE c.owner_id = ? OR c.customer_id = ?
		ORDER BY last_at DESC
	`, userID, userID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	conversations := []map[string]interface{}{}
	for rows.Next() {
		var id, listingID int
		var listingTitle, lastMessage string
		var updatedAt time.Time
		if err := rows.Scan(&id, &listingID, &listingTitle, &lastMessage, &updatedAt); err != nil {
			continue
		}
		conversations = append(conversations, map[string]interface{}{
			"id":            id,
			"listing_id":    listingID,
			"listing_title": listingTitle,
			"last_message":  lastMessage,
			"updated_at":    updatedAt,
		})
	}

	writeJSON(w, http.StatusOK, conversations)
}

func (h *MessageHandler) GetMessages(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	conversationID, err := strconv.Atoi(r.PathValue("id"))
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid conversation id"})
		return
	}

	var ownerID, customerID int
	err = h.DB.QueryRow("SELECT owner_id, customer_id FROM conversations WHERE id = ?", conversationID).Scan(&ownerID, &customerID)
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
		SELECT 
			m.id, 
			m.sender_id, 
			m.body, 
			m.created_at,
			COALESCE(NULLIF(TRIM(u.first_name), ''), u.email, u.phone, 'Пользователь') as sender_name
		FROM messages m
		JOIN users u ON u.id = m.sender_id
		WHERE m.conversation_id = ?
		ORDER BY m.created_at ASC
	`, conversationID)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer rows.Close()

	messages := []map[string]interface{}{}
	for rows.Next() {
		var id, senderID int
		var body, senderName string
		var createdAt time.Time
		if err := rows.Scan(&id, &senderID, &body, &createdAt, &senderName); err != nil {
			continue
		}
		messages = append(messages, map[string]interface{}{
			"id":          id,
			"sender_id":   senderID,
			"sender_name": senderName,
			"body":        body,
			"created_at":  createdAt,
		})
	}

	writeJSON(w, http.StatusOK, messages)
}

func (h *MessageHandler) SendMessage(w http.ResponseWriter, r *http.Request) {
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}

	conversationID, err := strconv.Atoi(r.PathValue("id"))
	if err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid conversation id"})
		return
	}

	var req struct {
		Message string `json:"message"`
	}
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid request"})
		return
	}

	message := strings.TrimSpace(req.Message)
	if message == "" {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "message cannot be empty"})
		return
	}

	var ownerID, customerID int
	err = h.DB.QueryRow("SELECT owner_id, customer_id FROM conversations WHERE id = ?", conversationID).Scan(&ownerID, &customerID)
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

	_, err = h.DB.Exec(`
		INSERT INTO messages (conversation_id, sender_id, body, created_at)
		VALUES (?, ?, ?, NOW())
	`, conversationID, userID, message)
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}

	writeJSON(w, http.StatusOK, map[string]string{"message": "message sent"})
}

func (h *MessageHandler) DeleteConversation(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodDelete {
		writeJSON(w, http.StatusMethodNotAllowed, map[string]string{"error": "method not allowed"})
		return
	}
	userID, ok := r.Context().Value("user_id").(int)
	if !ok {
		writeJSON(w, http.StatusUnauthorized, map[string]string{"error": "unauthorized"})
		return
	}
	conversationID, err := strconv.Atoi(r.PathValue("id"))
	if err != nil || conversationID <= 0 {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "invalid conversation id"})
		return
	}
	var ownerID, customerID int
	err = h.DB.QueryRow("SELECT owner_id, customer_id FROM conversations WHERE id = ?", conversationID).Scan(&ownerID, &customerID)
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

	tx, err := h.DB.Begin()
	if err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	defer tx.Rollback()
	if _, err := tx.Exec("DELETE FROM messages WHERE conversation_id = ?", conversationID); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if _, err := tx.Exec("DELETE FROM conversations WHERE id = ?", conversationID); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	if err := tx.Commit(); err != nil {
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": err.Error()})
		return
	}
	writeJSON(w, http.StatusOK, map[string]string{"message": "conversation deleted"})
}
