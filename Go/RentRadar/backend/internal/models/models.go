package models

import (
	"time"
)

type User struct {
	ID           int    `json:"id" db:"id"`
	Email        string `json:"email" db:"email"`
	Phone        string `json:"phone,omitempty" db:"phone"`
	PasswordHash string `json:"-" db:"password_hash"`
	FirstName    string `json:"first_name,omitempty" db:"first_name"`
	LastName     string `json:"last_name,omitempty" db:"last_name"`
	Role         string `json:"role" db:"role"`
	IsBlocked    bool   `json:"is_blocked" db:"is_blocked"`
}
type RegisterRequest struct {
	Email     string `json:"email"`
	Phone     string `json:"phone"`
	Password  string `json:"password"`
	FirstName string `json:"first_name"`
	LastName  string `json:"last_name"`
}

type LoginRequest struct {
	Identifier string `json:"identifier"`
	Password   string `json:"password"`
}
type AuthResponse struct {
	Token     string `json:"token"`
	UserID    int    `json:"user_id"`
	Email     string `json:"email"`
	Phone     string `json:"phone"`
	FirstName string `json:"first_name"`
	LastName  string `json:"last_name"`
	Role      string `json:"role"`
}
type Listing struct {
	ID                int        `json:"id" db:"id"`
	UserID            int        `json:"user_id" db:"user_id"`
	Title             string     `json:"title" db:"title"`
	Description       string     `json:"description,omitempty" db:"description"`
	ListingType       string     `json:"listing_type" db:"listing_type"`
	Price             float64    `json:"price" db:"price"`
	Area              float64    `json:"area,omitempty" db:"area"`
	Rooms             int        `json:"rooms,omitempty" db:"rooms"`
	Floor             int        `json:"floor,omitempty" db:"floor"`
	TotalFloors       int        `json:"total_floors,omitempty" db:"total_floors"`
	Address           string     `json:"address" db:"address"`
	City              string     `json:"city" db:"city"`
	District          string     `json:"district,omitempty" db:"district"`
	AvailableFrom     *time.Time `json:"available_from,omitempty" db:"available_from"`
	Deposit           string     `json:"deposit,omitempty" db:"deposit"`
	UtilitiesIncluded bool       `json:"utilities_included" db:"utilities_included"`
	Photos            string     `json:"photos,omitempty" db:"photos"`
	IsActive          bool       `json:"is_active" db:"is_active"`
	ModerationStatus  string     `json:"moderation_status" db:"moderation_status"`
	ViewsCount        int        `json:"views_count" db:"views_count"`
	Latitude          *float64   `json:"latitude,omitempty" db:"latitude"`
	Longitude         *float64   `json:"longitude,omitempty" db:"longitude"`
}
type Favorite struct {
	UserID    int `json:"user_id" db:"user_id"`
	ListingID int `json:"listing_id" db:"listing_id"`
}

type Booking struct {
	ID        int       `json:"id" db:"id"`
	ListingID int       `json:"listing_id" db:"listing_id"`
	UserID    int       `json:"user_id" db:"user_id"`
	CheckIn   time.Time `json:"check_in" db:"check_in"`
	CheckOut  time.Time `json:"check_out" db:"check_out"`
	Status    string    `json:"status" db:"status"`
}

type Review struct {
	ID               int       `json:"id" db:"id"`
	ListingID        int       `json:"listing_id" db:"listing_id"`
	UserID           int       `json:"user_id" db:"user_id"`
	Rating           int       `json:"rating" db:"rating"`
	Comment          string    `json:"comment,omitempty" db:"comment"`
	ModerationStatus string    `json:"moderation_status,omitempty" db:"moderation_status"`
	CreatedAt        time.Time `json:"created_at" db:"created_at"`
	AuthorName       string    `json:"author_name,omitempty"`
}

type CreateListingRequest struct {
	Title             string   `json:"title"`
	Description       string   `json:"description"`
	ListingType       string   `json:"listing_type"`
	Price             float64  `json:"price"`
	Area              float64  `json:"area"`
	Rooms             int      `json:"rooms"`
	Floor             int      `json:"floor"`
	TotalFloors       int      `json:"total_floors"`
	Address           string   `json:"address"`
	City              string   `json:"city"`
	District          string   `json:"district"`
	AvailableFrom     string   `json:"available_from"`
	Deposit           string   `json:"deposit"`
	UtilitiesIncluded bool     `json:"utilities_included"`
	Photos            string   `json:"photos"`
	Latitude          *float64 `json:"latitude"`
	Longitude         *float64 `json:"longitude"`
}
type BookingCreateRequest struct {
	ListingID int    `json:"listing_id"`
	UserID    int    `json:"user_id"`
	CheckIn   string `json:"check_in"`
	CheckOut  string `json:"check_out"`
}

type Conversation struct {
	ID           int       `json:"id"`
	ListingID    int       `json:"listing_id"`
	OwnerID      int       `json:"owner_id"`
	CustomerID   int       `json:"customer_id"`
	ListingTitle string    `json:"listing_title,omitempty"`
	LastMessage  string    `json:"last_message,omitempty"`
	LastAt       time.Time `json:"last_at"`
}

type ChatMessage struct {
	ID             int       `json:"id"`
	ConversationID int       `json:"conversation_id"`
	SenderID       int       `json:"sender_id"`
	SenderName     string    `json:"sender_name,omitempty"`
	Body           string    `json:"body"`
	CreatedAt      time.Time `json:"created_at"`
}
