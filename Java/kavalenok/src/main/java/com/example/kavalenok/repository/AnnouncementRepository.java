package com.example.kavalenok.repository;

import com.example.kavalenok.model.Announcement;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;

import java.util.List;

public interface AnnouncementRepository extends JpaRepository<Announcement, Long> {
    @Query("""
            SELECT a
            FROM Announcement a
            JOIN FETCH a.author author
            WHERE a.isActive = true
            ORDER BY a.createdAt DESC
            """)
    List<Announcement> findAllByIsActiveTrueOrderByCreatedAtDesc();
}