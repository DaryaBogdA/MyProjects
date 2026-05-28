package database

import (
	"database/sql"
	"log"
	"strings"
)

func EnsureSchema(db *sql.DB) {
	_, err := db.Exec(`CREATE TABLE IF NOT EXISTS user_reports (
		id INT AUTO_INCREMENT PRIMARY KEY,
		reporter_id INT NOT NULL,
		reported_user_id INT NOT NULL,
		listing_id INT NULL,
		reason TEXT NOT NULL,
		status VARCHAR(32) NOT NULL DEFAULT 'pending',
		created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
		INDEX idx_user_reports_status (status),
		INDEX idx_user_reports_listing (listing_id)
	)`)
	if err != nil {
		log.Printf("migrate user_reports: %v", err)
	}
	addColumn(db, "listings", "property_type", "VARCHAR(32) NOT NULL DEFAULT ''")
	addColumn(db, "listings", "plot_area", "DOUBLE NULL DEFAULT NULL")
}
func addColumn(db *sql.DB, table, column, def string) {
	_, err := db.Exec("ALTER TABLE " + table + " ADD COLUMN " + column + " " + def)
	if err != nil && !strings.Contains(strings.ToLower(err.Error()), "duplicate column") {
		log.Printf("migrate %s.%s: %v", table, column, err)
	}
}
