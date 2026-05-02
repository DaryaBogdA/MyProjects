package main

import (
	"log"
	"net/http"
	"path/filepath"

	"rentradar/backend/internal/app"
	"rentradar/backend/internal/database"

	"github.com/joho/godotenv"
)

func main() {
	if err := godotenv.Load(); err != nil {
		log.Println("Warning: No .env file found, using system environment variables")
	}

	cfg := database.LoadConfigFromEnv()
	db, err := database.OpenMySQL(cfg)
	if err != nil {
		log.Fatalf("database connection failed: %v", err)
	}
	defer db.Close()

	log.Println("database connected successfully")

	projectRoot := filepath.Join("..")
	router := app.NewRouter(db, projectRoot)

	addr := ":5000"
	log.Printf("RentRadar backend started on %s\n", addr)
	if err := http.ListenAndServe(addr, router); err != nil {
		log.Fatal(err)
	}
}
